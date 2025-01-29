using System.Collections.Generic;
using GTA;
using Waldhari.Common.Behavior.Mission;
using Waldhari.Common.Behavior.Ped;
using Waldhari.Common.Entities;

namespace Waldhari.CokeWork.Missions
{
    [ScriptAttributes(NoDefaultInstance = true)]
    public class CokeWorkPickUpPedScript : GenericPickUpPedMissionScript
    {
        public CokeWorkPickUpPedScript() 
            : base("CokeWorkPickUpPedScript", "cokework_pickupped_success") { }


        protected override WPosition Destination => CokeWorkHelper.Positions.Parking;
        protected override void ShowStartedMessage()
        {
            CokeWorkHelper.ShowFromContact("cokework_pickupped_started");
        }
        protected override string PedMessageKey => "supervisor";
        protected override string FailPedDeadMessageKey => "cokework_pickupped_fail_dead";
        protected override string RendezvousMessageKey => "cokework_pickupped_step_rendezvous";
        protected override string WaitMessageKey => "cokework_pickupped_step_wait";
        protected override string DriveMessageKey => "cokework_pickupped_step_drive";
        protected override PedHash PedHash => PedHash.WeedMale01;
        protected override string DestinationMessageKey => "cokework_parking";
        protected override List<string> EndComplement()
        {
            CokeWorkSave.Instance.Worker = true;
            CokeWorkSave.Instance.Save();
            
            return null;
        }

        protected override void SendPed(PedActingScript script)
        {
            CokeWorkHelper.ManufactureScript = InstantiateScript<CokeWorkManufactureScript>();
            CokeWorkHelper.ManufactureScript.WorkerScript = script;
            CokeWorkHelper.StartManufacture(true);
        }
    }
}