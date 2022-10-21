using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Valve.VR.InteractionSystem
{
    public class TeleportArea_Invisible : TeleportMarkerBase
    {
        public override void Highlight(bool highlight)
        {
        }

        public override void SetAlpha(float tintAlpha, float alphaPercent)
        {
        }

        public override bool ShouldActivate(Vector3 playerPosition)
        {
            return true;
        }

        public override bool ShouldMovePlayer()
        {
            return true;
        }

        public override void UpdateVisuals()
        {
        }
    }
}
