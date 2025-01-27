using System.Collections.Generic;
using iFruitAddon2;
using Waldhari.Common.Behavior.Property;
using Waldhari.Common.Files;
using Waldhari.Common.Misc;

namespace Waldhari.Common.UI
{
    public static class PhoneHelper
    {
        private static readonly Dictionary<Property.Owner, CustomiFruit> Phones = new Dictionary<Property.Owner, CustomiFruit>();

        public static CustomiFruit GetCharacterPhone()
        {
            var character = PlayerHelper.GetCharacterId();
            if(Phones.ContainsKey(character)) return Phones[character];
            
            Phones.Add(character, new CustomiFruit());
            
            Logger.Debug($"iFuit created for {character}; number of contact={Phones[character].Contacts.Count}");
            
            return Phones[character];
        }

        public static void ManageContact(iFruitContact contact, Property.Owner owner)
        {
            var character = PlayerHelper.GetCharacterId();
            if (character != owner && !GlobalOptions.Instance.UniversalBusinesses)
            {
                if (ContactExists(contact))
                {
                    Logger.Debug(
                        $"Current player ='{character}', " +
                        $"Contact='{contact.Name}' exists but player is not owner='{owner}' : " +
                        $"removing contact.");
                    if (!GetCharacterPhone().Contacts.Remove(contact))
                    {
                        Logger.Warning(
                            $"Current player ='{character}', " +
                            $"Contact='{contact.Name}' exists but player is not owner='{owner}' : " +
                            $"can not remove contact!");
                    }
                    Logger.Debug($"number of contact={Phones[character].Contacts.Count}");
                }
            }
            else
            {
                if (!ContactExists(contact))
                {
                    Logger.Debug(
                        $"Current player ='{character}', " +
                        $"Contact='{contact.Name}' does not exist but player is owner='{owner}' : " +
                        $"adding contact.");
                    var total = GetCharacterPhone().Contacts.Count;
                    GetCharacterPhone().Contacts.Add(contact);
                    // should have +1
                    if (total == GetCharacterPhone().Contacts.Count)
                    {
                        Logger.Warning(
                            $"Current player ='{character}', " +
                            $"Contact='{contact.Name}' does not exist but player is owner='{owner}' : " +
                            $"can not add contact!");
                    }
                    Logger.Debug($"number of contact={Phones[character].Contacts.Count}");
                }
            }
        }

        private static bool ContactExists(iFruitContact contact)
        {
            return GetCharacterPhone().Contacts.IndexOf(contact) >= 0;
        }
        
    }
}