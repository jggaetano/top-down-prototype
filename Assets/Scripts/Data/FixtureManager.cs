using System.Collections.Generic;
using UnityEngine;

public class FixtureManager  {

    AreaController controller { get { return AreaController.Instance; } }
    Area area { get { return AreaController.Instance.area; } }
    Dictionary<GameObject, Fixture> goToFxMap { get { return FixtureSpriteController.Instance.gameObjectFixtureMap;  } }

    public FixtureManager() {
    }

    public void Interact(GameObject fixture) {

        if (goToFxMap.ContainsKey(fixture) == false)
        {
            Debug.LogError("Interact: No GameOject " + fixture + " in gameOjectFixtureMap");
            return;
        }

        Fixture f = goToFxMap[fixture];
        f.Interact();

    }

    public void WalkThroughDoor(GameObject fixture /*Tile tile*/)
    {

        //Fixture fixture = GetFixtureInTile(tile);
        //if (fixture == null)
        //    return;
        //if (fixture.GetType() != typeof(Door))
        //    return;

        if (goToFxMap.ContainsKey(fixture) == false)
        {
            Debug.LogError("WalkThroughDoor: No GameOject in gameOjectFixtureMap");
            return;
        }

        Fixture f = goToFxMap[fixture];
        if (f.GetType() == typeof(Door))
        {
            if ((f as Door).LinkToArea != area.Place)
            {
                AreaController.nextAreaDoor = (f as Door).LinkToDoor;
                controller.NextArea((f as Door).LinkToArea);
            }
            else
            {
                Door nextDoor = area.GetDoor((f as Door).LinkToDoor);
                area.hero.Move(nextDoor.ExitX, nextDoor.ExitY);
            }
        }

    }

    public void TripSensor(GameObject fixture) {

        if (goToFxMap.ContainsKey(fixture) == false)
        {
            Debug.LogError("TripSensor: No GameOject in gameOjectFixtureMap");
            return;
        }

        Fixture f = goToFxMap[fixture];
        f.Interact();

        if (f.GetType() == typeof(Door))
            (f as Door).SensorState = true;
        
    }

    public void SensorClear(GameObject fixture) {

        if (goToFxMap.ContainsKey(fixture) == false)
        {
            Debug.LogError("TripSensor: No GameOject in gameOjectFixtureMap");
            return;
        }

        Fixture f = goToFxMap[fixture];
        if (f.GetType() == typeof(Door))
            (f as Door).SensorState = false;
    }

    Fixture GetFixtureInTile(Tile tile) {

        foreach (Fixture fixture in area.fixtures) {
            if (tile == fixture.MyTile)
                return fixture;
        }
        return null;

    }

}
