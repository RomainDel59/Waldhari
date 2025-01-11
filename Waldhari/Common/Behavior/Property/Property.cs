using GTA;
using GTA.Math;
using Waldhari.Common.Entities;
using Waldhari.Common.Entities.Helpers;
using Waldhari.Common.Misc;

namespace Waldhari.Common.Behavior.Property
{
    public class Property
    {
        
        public enum Owner
        {
            None = 0,
            Michael = 1,
            Trevor = 2,
            Franklin = 3
        }
        
        public Vector3 Position;
        public bool IsPlayerNear() => WPositionHelper.IsNear(Position, Game.Player.Character.Position, 5);
        
        public string NameKey;
        
        public int Price;
        public bool IsPlayerCanBuy() => Game.Player.Money >= Price;
        
        public BlipSprite Sprite = BlipSprite.Business;
        
        public Owner Holder = Owner.None;
        public bool IsOwned() => Holder != Owner.None;
        public bool IsPlayerOwner() => Holder == PlayerHelper.GetCharacterId();

        private WBlip _blip;

        public void ShowBlip()
        {
            if (!GlobalOptions.Instance.ShowBlips) return;

            _blip?.Remove();
            _blip = WBlipHelper.CreateProperty(
                Sprite,
                Position,
                Holder,
                NameKey,
                Price
            );
        }

    }
}