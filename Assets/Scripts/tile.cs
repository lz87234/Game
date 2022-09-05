using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Global;

public class CustomTile {
  public static ConcurrentDictionary<Vector2Int, CustomTile> tiles = new();

  public Tile tile { get; private set; }
  public Vector2Int pos { get; private set; }
  public Vector2 offset { get; private set; }
  public Tilemap tilemap { get; private set; }
  public float dir { get; private set; }
  public bool discarded { get; private set; }

  public string type { get => tile.ToString(); }
  public bool moved;

  public CustomTile(Tilemap tilemap, Tile tile, Vector2Int pos = new(), float dir = 0f, Vector2 offset = new()) {
    this.tile = tile;
    this.pos = pos;
    this.dir = dir;
    this.tilemap = tilemap;
    this.offset = offset;
    Render();
    tiles[pos] = this;
    last = this;
  }

  public void SetTile(Tile tile) {
    this.tile = tile;
    Render();
  }

  public void SetOffset(Vector2 offset) {
    this.offset = offset;
    Render();
  }

  public void SetDirection(float dir) {
    this.dir = dir;
    Render();
  }

  public void SetPosition(Vector2Int? pos_) {
    if (pos_ is Vector2Int pos) {
      if (tiles.ContainsKey(pos)) {
        tiles[pos].SetPosition(null);
      }
      Remove();
      this.pos = pos;
      tiles[pos] = this;
      Render();
    } else {
      tiles.Remove(this.pos, out _);
    }
  }

  public void SetTilemap(Tilemap tilemapnew) {
    Remove();
    this.tilemap = tilemapnew;
    Render();
  }

  public void Destroy() {
    Remove();
    discarded = true;
  }

  ~CustomTile() {
    Destroy();
  }

  private Vector3Int getPos3() {
    return new Vector3Int(pos.x, pos.y, 0);
  }
  
  private void Render() {
    tilemap.SetTile(getPos3(), tile);
    tilemap.SetTransformMatrix(getPos3(), Matrix4x4.TRS(offset, Quaternion.Euler(0f, 0f, dir), Vector3.one));
  }

  private void Remove() {
    tilemap.SetTile(getPos3(), null);
    tiles.Remove(this.pos, out _);
  }
}
