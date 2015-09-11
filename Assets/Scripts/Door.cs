public class Door {
	
	public int x, y;
	public int zoneFrom;
	public int zoneTo;
	public int doorDirection; // 0: horizontal, 1 vertical
	MapCell [,] map;
	
	public Door (int x, int y, int zoneFrom, MapCell[,] map) {
		this.x = x;
		this.y = y;
		this.zoneFrom = zoneFrom;
		this.map = map;
		zoneTo = 0;
		doorDirection = -1;
	}
		
	public void updateDoorDirection () {
		this.doorDirection = (map[x - 1, y].cellKind == MapCell.CellKind.WALKABLE) ? 0 : 1;
	}
		
	public void updateDoorZones () {
		if (doorDirection == 0)
			this.zoneTo = (map[x - 1, y].zoneID == zoneFrom) ? map[x + 1, y].zoneID : map[x - 1, y].zoneID;
		else
			this.zoneTo = (map[x, y - 1].zoneID == zoneFrom) ? map[x, y + 1].zoneID : map[x, y - 1].zoneID;
	}
}