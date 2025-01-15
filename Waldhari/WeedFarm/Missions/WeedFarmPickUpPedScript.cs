using System.Collections.Generic;
using GTA;
using Waldhari.Common.Behavior.Mission;
using Waldhari.Common.Behavior.Ped;
using Waldhari.Common.Entities;

namespace Waldhari.WeedFarm.Missions
{
    [ScriptAttributes(NoDefaultInstance = true)]
    public class WeedFarmPickUpPedScript : GenericPickUpPedMissionScript
    {
        public WeedFarmPickUpPedScript() 
            : base("WeedFarmPickUpPedScript", "weedfarm_pickupped_success") { }


        protected override WPosition Destination => WeedFarmHelper.Positions.Parking;
        protected override WPosition Workstation => WeedFarmHelper.Positions.GetWorkstation();
        protected override void ShowStartedMessage()
        {
            WeedFarmHelper.ShowFromContact("weedfarm_pickupped_started");
        }
        protected override string PedMessageKey => "chemist";
        protected override string FailPedDeadMessageKey => "weedfarm_pickupped_fail_dead";
        protected override string RendezvousMessageKey => "weedfarm_pickupped_step_rendezvous";
        protected override string WaitMessageKey => "weedfarm_pickupped_step_wait";
        protected override string DriveMessageKey => "weedfarm_pickupped_step_drive";
        protected override PedHash PedHash => PedHash.WeedMale01;
        protected override string DestinationMessageKey => "weedfarm_parking";
        protected override List<string> EndComplement()
        {
            WeedFarmSave.Instance.Worker = true;
            WeedFarmSave.Instance.Save();
            
            return null;
        }

        protected override void SendPed(PedActingScript script)
        {
            WeedFarmHelper.ManufactureScript = InstantiateScript<WeedFarmManufactureScript>();
            WeedFarmHelper.ManufactureScript.WorkerScript = script;
            WeedFarmHelper.StartManufacture(true);
        }
    }
}