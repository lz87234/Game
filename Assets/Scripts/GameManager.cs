using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Global;

public class GameManager : MonoBehaviour {
  public Tilemap tilemap;
  public new Camera camera;

  [SerializeField]
  private List<Tile> TileAssets;

  [SerializeField]
  private TilesGetter tiles;
  private class TilesGetter {
    private List<Tile> tiles;
    public TilesGetter(List<Tile> tiles) {
      this.tiles = tiles;
    }
    public Tile this[int index] { get => tiles[index]; }
    public Tile this[string name] { get => tiles.FirstOrDefault(x => x.ToString() == name); }
  }

  private void Start() {
    tiles = new(TileAssets);
    new CustomTile(tilemap, tiles["mover"], new(0, 0), 90f);
    new CustomTile(tilemap, tiles["slide"], new(0, 1), 90f);
    InvokeRepeating("Tick", 0.2f, 0.2f);
  }

  private void OnMouseUp() {
    var pos = camera.ScreenToWorldPoint(Input.mousePosition);
    pos = new(Mathf.Floor(pos.x), Mathf.Ceil(pos.y), 1);
  }

  private void Tick() {
    foreach (var tile in CustomTile.tiles.Values) {
      if (tile.discarded) continue;
      switch (tile.type) {
        case "mover":
          if (tile.moved) { tile.moved = false; break; }
          Dictionary<Vector2Int, CustomTile> pushed = new();
          var t = tile;
          while (true) {
            if (t == null) {
              foreach (var (y, x) in pushed.OrderByDescending(v => v.Key.x == 0 ? v.Key.y : v.Key.x))
                x.SetPosition(y);
            }

            switch (tile.type) {
              case "immobile":
                pushed = new();
                t = null;
                break;

              case "slider":
                if (Vector2Int.FloorToInt(Quaternion.Euler(0, 0, t.dir) * Vector2.right) !=
                    Vector2Int.FloorToInt(Quaternion.Euler(0, 0, tile.dir) * Vector2.right)) {
                  pushed = new();
                  t = null;
                }
                break;
            }

            if (t == null) break;
            if (t != tile) t.moved = true;
            var newPOS = t.pos + Vector2Int.CeilToInt(new(Mathf.Cos(tile.dir*Mathf.Deg2Rad), Mathf.Sin(tile.dir*Mathf.Deg2Rad)));
            pushed[newPOS] = t;
            t = CustomTile.tiles.GetValueOrDefault(newPOS, null);
          }
          break;


      }
    }
  }
}
