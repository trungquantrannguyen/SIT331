namespace robot_controller_api.Persistence;

public interface IMapDataAccess
{
    List<Map> GetMaps();

    Map? GetMapById(int id);

    List<Map> GetSquareMaps();

    Map AddMap(Map newMap);

    Map? UpdateMap(int id, Map updatedMap);

    bool DeleteMap(int id);

    bool IsCoordinateOnMap(int mapId, int x, int y);
}