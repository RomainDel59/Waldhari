using iFruitAddon2;
using Waldhari.Behavior.Property;
using Waldhari.Common.Files;
using Waldhari.Common.Misc;

namespace Waldhari.Common.UI
{
    public static class PhoneHelper
    {
        private static CustomiFruit _iFruit;

        public static CustomiFruit GetIFruit()
        {
            if(_iFruit != null) return _iFruit;
            
            _iFruit = new CustomiFruit();
            return _iFruit;
        }

        public static void ManageContact(iFruitContact contact, Property.Owner owner)
        {
            if (PlayerHelper.GetCharacterId() == owner)
            {
                if (!ContactExists(contact))
                {
                    Logger.Debug("Contact does not exist but player is owner : adding contact.");
                    GetIFruit().Contacts.Add(contact);
                }
            }
            else
            {
                if (ContactExists(contact))
                {
                    Logger.Debug("Contact exists but player is not owner : removing contact.");
                    GetIFruit().Contacts.Remove(contact);
                }
            }
        }

        private static bool ContactExists(iFruitContact contact)
        {
            return GetIFruit().Contacts.IndexOf(contact) >= 0;
        }
        
    }
}