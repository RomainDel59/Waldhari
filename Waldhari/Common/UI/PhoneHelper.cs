using iFruitAddon2;
using Waldhari.Common.Behavior.Property;
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
            
            Logger.Debug($"iFuit created num contacts={_iFruit.Contacts.Count}");
            
            return _iFruit;
        }

        public static void ManageContact(iFruitContact contact, Property.Owner owner)
        {
            if (PlayerHelper.GetCharacterId() == owner)
            {
                if (!ContactExists(contact))
                {
                    Logger.Debug($"Current player ='{PlayerHelper.GetCharacterId()}', Contact='{contact.Name}' does not exist but player is owner='{owner}' : adding contact.");
                    GetIFruit().Contacts.Add(contact);
                }
            }
            else
            {
                if (ContactExists(contact))
                {
                    Logger.Debug($"Current player ='{PlayerHelper.GetCharacterId()}', Contact='{contact.Name}' exists but player is not owner='{owner}' : removing contact.");
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