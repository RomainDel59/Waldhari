﻿using GTA;
using Waldhari.Common.Entities.Helpers;
using Waldhari.Common.Exceptions;
using Waldhari.Common.Files;
using Waldhari.Common.UI;

namespace Waldhari.Common.Entities
{
    public class WVehicle
    {
        public Vehicle Vehicle = null;

        public WPosition InitialPosition = null;
        
        public VehicleHash VehicleHash = default;
        
        public WBlip WBlip = null;

        /// <summary>
        /// Creates the vehicle according entered properties.
        /// If the vehicle has already been created, does nothing.
        /// If the model cannot be loaded, error will be logged.
        /// Blip is not created if required.
        /// </summary>
        /// <exception cref="TechnicalException">If at least one of required property is empty</exception>
        public void Create()
        {
            if (Vehicle != null) return;
            
            if(InitialPosition == null) throw new TechnicalException("InitialPosition cannot be empty");
            if(VehicleHash == default) throw new TechnicalException("VehicleHash cannot be empty");

            Logger.Debug($"VehicleHash: {VehicleHash}");
            Logger.Debug($"InitialPosition.Position: X={InitialPosition.Position.X}, Y={InitialPosition.Position.Y}, Z={InitialPosition.Position.Z}");
            
            Model model = VehicleHash;
            Logger.Debug($"model: {model}");
            
            if (!model.Request(2000))
            {
                if(!model.Request(2000)) throw new TechnicalException("Request PedHash failed");
            }
            
            Logger.Debug($"World.VehicleCount '{World.VehicleCount}' >= World.VehicleCapacity '{World.VehicleCapacity}' ('{World.VehicleCount >= World.VehicleCapacity}') || !model.IsVehicle '{!model.IsVehicle}' || !model.Request(1000) '{!model.Request(1000)}'");
            
            Vehicle = World.CreateVehicle(model, InitialPosition.Position, InitialPosition.Heading);
            model.MarkAsNoLongerNeeded();
            
            Logger.Debug($"Created Vehicle: {Vehicle}");

            MoveInPosition();
        }

        /// <summary>
        /// Removes the vehicle. It deletes it, not only mark as no longer needed. 
        /// If the vehicle has already been removed, does nothing.
        /// Properties of WVehicle class are preserved,
        /// only the vehicle is delete and its property set to null.
        /// Do the same to the WBlip if exists.
        /// </summary>
        public void Remove()
        {
            WBlip?.Remove();
            Vehicle?.Delete();
            Vehicle = null;
        }
        
        /// <summary>
        /// Moves the vehicle immediately in the initial position.
        /// </summary>
        public void MoveInPosition()
        {
            if(InitialPosition == null) throw new TechnicalException("InitialPosition cannot be empty");
            if(Vehicle == null) throw new TechnicalException("Vehicle cannot be empty");
            
            Vehicle.PositionNoOffset = InitialPosition.Position;
            Vehicle.Rotation = InitialPosition.Rotation;
            Vehicle.Heading = InitialPosition.Heading;
        }

        public void MakeMissionDestination(string nameKey)
        {
            if(WBlip == null)
            {
                WBlip = WBlipHelper.GetMission(nameKey);
                WBlip.Vehicle = Vehicle;
            }
            
            WBlip.Create();
            
            MarkerHelper.DrawEntityMarkerOnBlip(WBlip);
        }

        public void RemoveMissionDestination()
        {
            WBlip?.Remove();
        }
    }
}