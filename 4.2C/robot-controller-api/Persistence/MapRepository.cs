using Npgsql;

namespace robot_controller_api.Persistence;

public class MapRepository : IMapDataAccess, IRepository
{
    private IRepository Repo => this;

    public List<Map> GetMaps()
    {
        return Repo.ExecuteReader<Map>(
            "SELECT id, columns, rows, \"Name\", description, createddate, modifieddate FROM public.map ORDER BY id"
        );
    }

    public Map? GetMapById(int id)
    {
        var sqlParams = new NpgsqlParameter[]
        {
            new("id", id)
        };

        return Repo.ExecuteReader<Map>(
            "SELECT id, columns, rows, \"Name\", description, createddate, modifieddate FROM public.map WHERE id = @id",
            sqlParams
        ).SingleOrDefault();
    }

    public List<Map> GetSquareMaps()
    {
        return Repo.ExecuteReader<Map>(
            "SELECT id, columns, rows, \"Name\", description, createddate, modifieddate FROM public.map WHERE columns = rows ORDER BY id"
        );
    }

    public Map AddMap(Map newMap)
    {
        var sqlParams = new NpgsqlParameter[]
        {
            new("columns", newMap.Columns),
            new("rows", newMap.Rows),
            new("name", newMap.Name),
            new("description", newMap.Description ?? (object)DBNull.Value)
        };

        return Repo.ExecuteReader<Map>(
            """
            INSERT INTO public.map 
            (columns, rows, "Name", description, createddate, modifieddate)
            VALUES (@columns, @rows, @name, @description, current_timestamp, current_timestamp)
            RETURNING id, columns, rows, "Name", description, createddate, modifieddate;
            """,
            sqlParams
        ).Single();
    }

    public Map? UpdateMap(int id, Map updatedMap)
    {
        var sqlParams = new NpgsqlParameter[]
        {
            new("id", id),
            new("columns", updatedMap.Columns),
            new("rows", updatedMap.Rows),
            new("name", updatedMap.Name),
            new("description", updatedMap.Description ?? (object)DBNull.Value)
        };

        return Repo.ExecuteReader<Map>(
            """
            UPDATE public.map
            SET columns = @columns,
                rows = @rows,
                "Name" = @name,
                description = @description,
                modifieddate = current_timestamp
            WHERE id = @id
            RETURNING id, columns, rows, "Name", description, createddate, modifieddate;
            """,
            sqlParams
        ).SingleOrDefault();
    }

    public bool DeleteMap(int id)
    {
        var sqlParams = new NpgsqlParameter[]
        {
            new("id", id)
        };

        var affectedRows = Repo.ExecuteNonQuery(
            "DELETE FROM public.map WHERE id = @id",
            sqlParams
        );

        return affectedRows > 0;
    }
}