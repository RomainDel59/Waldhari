using System.Collections.Generic;
using GTA;
using Waldhari.Common.Behavior.Mission;
using Waldhari.Common.Behavior.Ped;
using Waldhari.Common.Entities;
using Waldhari.Common.UI;

namespace Waldhari.MethLab.Missions
{
    [ScriptAttributes(NoDefaultInstance = true)]
    public class MethLabPickUpPedScript : GenericPickUpPedMissionScript
    {
        public MethLabPickUpPedScript() 
            : base("MethLabPickUpPedScript", "methlab_pickupped_success") { }


        protected override WPosition Destination => MethLabHelper.Positions.Parking;
        protected override WPosition Workstation => MethLabHelper.Positions.GetWorkstation();
        protected override void ShowStartedMessage()
        {
            NotificationHelper.ShowFromRon("methlab_pickupped_started");
        }
        protected override string PedMessageKey => "chemist";
        protected override string FailPedDeadMessageKey => "methlab_pickupped_fail_dead";
        protected override string RendezvousMessageKey => "methlab_pickupped_step_rendezvous";
        protected override string WaitMessageKey => "methlab_pickupped_step_wait";
        protected override string DriveMessageKey => "methlab_pickupped_step_drive";
        protected override PedHash PedHash => PedHash.MethMale01;
        protected override string DestinationMessageKey => "methlab_parking";
        protected override List<string> EndComplement()
        {
            MethLabSave.Instance.Worker = true;
            MethLabSave.Instance.Save();
            
            return null;
        }

        protected override void SendPed(PedActingScript script)
        {
            MethLabHelper.ManufactureScript = InstantiateScript<MethLabManufactureScript>();
            MethLabHelper.ManufactureScript.WorkerScript = script;
            MethLabHelper.StartManufacture(true);
        }
    }
}