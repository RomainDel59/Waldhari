using Common.Entities;
using GTA;

namespace GTAVMods.Utils
{
    public abstract class AbstractEntity
    {
        protected WBlip _wBlip;
        protected Entity Entity;

        public void AttachMissionBlip(string nameKey)
        {
            if (_wBlip != null) return;

            _wBlip = new WBlip(nameKey);
            _wBlip.CreateOnEntity(Entity);
        }

        public void RemoveBlip()
        {
            if (_wBlip == null) return;

            _wBlip.Remove();
            _wBlip = null;
        }

        public virtual void Remove()
        {
            if (Entity == null) return;

            RemoveBlip();
            Entity.Delete();
            Entity = null;
        }
    }
}