using System.Numerics;

namespace SoundArcade.Abstractions;

public interface IRenderer
{
  void Clear(RgbaColor color);
  void DrawBox(Vector3 center, Vector3 size, RgbaColor color);
  void DrawSphere(Vector3 center, float radius, RgbaColor color);
  void DrawLine(Vector3 start, Vector3 end, RgbaColor color);
  void DrawPoint(Vector3 position, RgbaColor color);
}
