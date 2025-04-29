namespace robot_controller_api.Persistence;

public interface IMapDataAccess
{
    void DeleteMap(int id);
    Map GetMapById(int id);
    List<Map> GetMaps();
    Map InsertMap(string name, string? description, int columns, int rows);
    Map UpdateMap(int id, string name, string? description, int columns, int rows);
}