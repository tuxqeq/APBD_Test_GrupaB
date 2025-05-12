using APBD_test_grupaB.DTOs;
using APBD_test_grupaB.models;
using Microsoft.Data.SqlClient;

namespace APBD_test_grupaB.Services;

using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;


public class VisitService : IVisitService
{
    private readonly string _connectionString;

    public VisitService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<VisitResponseDto> GetVisitByIdAsync(int visitId)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var visitQuery = @"
            SELECT v.date, c.first_name, c.last_name, c.date_of_birth, m.mechanic_id, m.licence_number
            FROM Visit v
            JOIN Client c ON v.client_id = c.client_id
            JOIN Mechanic m ON v.mechanic_id = m.mechanic_id
            WHERE v.visit_id = @VisitId";

        using var visitCommand = new SqlCommand(visitQuery, connection);
        visitCommand.Parameters.AddWithValue("@VisitId", visitId);

        using var reader = await visitCommand.ExecuteReaderAsync();

        if (!reader.HasRows)
        {
            return null;
        }

        await reader.ReadAsync();

        var visitResponse = new VisitResponseDto
        {
            Date = reader.GetDateTime(0),
            Client = new ClientDto
            {
                FirstName = reader.GetString(1),
                LastName = reader.GetString(2),
                DateOfBirth = reader.GetDateTime(3)
            },
            Mechanic = new MechanicDto
            {
                MechanicId = reader.GetInt32(4),
                LicenceNumber = reader.GetString(5)
            },
            VisitServices = new List<VisitServiceDto>()
        };

        reader.Close();

        var servicesQuery = @"
            SELECT s.name, vs.service_fee
            FROM Visit_Service vs
            JOIN Service s ON vs.service_id = s.service_id
            WHERE vs.visit_id = @VisitId";

        using var servicesCommand = new SqlCommand(servicesQuery, connection);
        servicesCommand.Parameters.AddWithValue("@VisitId", visitId);

        using var servicesReader = await servicesCommand.ExecuteReaderAsync();

        while (await servicesReader.ReadAsync())
        {
            visitResponse.VisitServices.Add(new VisitServiceDto
            {
                Name = servicesReader.GetString(0),
                ServiceFee = servicesReader.GetDecimal(1)
            });
        }

        return visitResponse;
    }

    public async Task<VisitCreateResult> CreateVisitAsync(VisitCreateDto dto, CancellationToken cancellationToken)
    {
        await using var con = new SqlConnection(_connectionString);
        await con.OpenAsync(cancellationToken);

        await using (var checkVisitCmd = new SqlCommand("SELECT COUNT(1) FROM Visit WHERE visit_id = @VisitId", con))
        {
            checkVisitCmd.Parameters.AddWithValue("@VisitId", dto.VisitId);

            var exists = (int)await checkVisitCmd.ExecuteScalarAsync(cancellationToken) > 0;

            if (exists)
                return VisitCreateResult.AlreadyExists;
        }

        await using (var checkClientCmd = new SqlCommand("SELECT COUNT(1) FROM Client WHERE client_id = @ClientId", con))
        {
            checkClientCmd.Parameters.AddWithValue("@ClientId", dto.ClientId);

            var exists = (int)await checkClientCmd.ExecuteScalarAsync(cancellationToken) > 0;

            if (!exists)
                return VisitCreateResult.ClientNotFound;
        }

        int? mechanicId = null;
        await using (var getMechanicCmd = new SqlCommand("SELECT mechanic_id FROM Mechanic WHERE licence_number = @LicenceNumber", con))
        {
            getMechanicCmd.Parameters.AddWithValue("@LicenceNumber", dto.MechanicLicenceNumber);

            var result = await getMechanicCmd.ExecuteScalarAsync(cancellationToken);

            if (result == null)
                return VisitCreateResult.MechanicNotFound;
            mechanicId = (int)result;
        }

        var serviceIds = new Dictionary<string, int>();
        foreach (var service in dto.Services)
        {
            await using var checkServiceCmd = new SqlCommand("SELECT service_id FROM Service WHERE name = @Name", con);
            checkServiceCmd.Parameters.AddWithValue("@Name", service.Name);
            var result = await checkServiceCmd.ExecuteScalarAsync(cancellationToken);

            if (result == null)
                return VisitCreateResult.ServiceNotFound;

            serviceIds[service.Name] = (int)result;
        }

        await using (var insertVisitCmd = new SqlCommand(@"
            INSERT INTO Visit (visit_id, client_id, mechanic_id, date)
            VALUES (@VisitId, @ClientId, @MechanicId, @Date)", con))
        {
            insertVisitCmd.Parameters.AddWithValue("@VisitId", dto.VisitId);
            insertVisitCmd.Parameters.AddWithValue("@ClientId", dto.ClientId);
            insertVisitCmd.Parameters.AddWithValue("@MechanicId", mechanicId!.Value);
            insertVisitCmd.Parameters.AddWithValue("@Date", DateTime.Now);

            await insertVisitCmd.ExecuteNonQueryAsync(cancellationToken);
        }

        foreach (var service in dto.Services)
        {
            int serviceId = serviceIds[service.Name];

            await using var insertVisitServiceCmd = new SqlCommand(@"
                INSERT INTO Visit_Service (visit_id, service_id, service_fee)
                VALUES (@VisitId, @ServiceId, @Fee)", con);
            insertVisitServiceCmd.Parameters.AddWithValue("@VisitId", dto.VisitId);
            insertVisitServiceCmd.Parameters.AddWithValue("@ServiceId", serviceId);
            insertVisitServiceCmd.Parameters.AddWithValue("@Fee", service.ServiceFee);

            await insertVisitServiceCmd.ExecuteNonQueryAsync(cancellationToken);
        }

        return VisitCreateResult.Success;
    }
}