using Colin.Gimbal;
using Colin.Gimbal.Parts;
using Lidgren.Network;
using Lidgren.Network.Xna;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ultra
{
    public class NetworkProtocol
    {
        [AutoHook("Colin.Gimbal.NetworkVehicleSerializer", "SerializeComponentRecursive", Skip = true)]
        public static void SerializeComponentRecursive(Component comp, NetOutgoingMessage msg, int depth)
        {
            try
            {
                byte source = (byte)NetworkVehicleSerializer.GetTypeIDFromComponent(comp);
                msg.Write(source);
            }
            catch
            {
                msg.Write((byte)255);
                msg.Write(comp.GetType().FullName);
            }
            msg.Write((ushort)comp.ComponentID);
            Vector2 pos = comp.Physics.Position + Utility.Rotate(comp.Physics.CoMAdjustment, comp.Physics.Angle);
            DataJew.WriteAccuratePosition(msg, pos);
            msg.WriteHalfPrecision(comp.Physics.Velocity.X);
            msg.WriteHalfPrecision(comp.Physics.Velocity.Y);
            msg.Write(comp.BaseAngle);
            DataJew.WriteAccuratePosition(msg, comp.BasePosition);
            msg.Write(comp.Physics.Angle);
            msg.Write(comp.Physics.AngularVelocity);
            msg.Write(comp.HitPoints);
            msg.Write((ushort)comp.ChildAttachments.Count);
            foreach (Component current in comp.ChildAttachments)
            {
                SerializeComponentRecursive(current, msg, depth + 1);
            }
        }

        [AutoHook("Colin.Gimbal.NetworkVehicleSerializer", "DeserializeComponentRecursive", Skip = true)]
        public static Component DeserializeComponentRecursive(NetIncomingMessage msg, int depth)
        {
            Component componentFromTypeID;
            byte componentTypeID = msg.ReadByte();
            if (componentTypeID == 255)
            {
                string parttype = msg.ReadString();
                componentFromTypeID = (Component)ModLoader.CreateInstance(parttype);
            }
            else
            {
                componentFromTypeID = NetworkVehicleSerializer.GetComponentFromTypeID((int)componentTypeID);
            }
            int id = (int)msg.ReadUInt16();
            componentFromTypeID.Physics.Position = DataJew.ReadAccuratePosition(msg);
            componentFromTypeID.Physics.Velocity = new Vector2(msg.ReadHalfPrecisionSingle(), msg.ReadHalfPrecisionSingle());
            componentFromTypeID.BaseAngle = msg.ReadFloat();
            componentFromTypeID.BasePosition = DataJew.ReadAccuratePosition(msg);
            componentFromTypeID.Physics.Angle = msg.ReadFloat();
            componentFromTypeID.Physics.AngularVelocity = msg.ReadFloat();
            componentFromTypeID.HitPoints = msg.ReadFloat();
            int num = (int)msg.ReadUInt16();
            for (int i = 0; i < num; i++)
            {
                Component component = DeserializeComponentRecursive(msg, depth + 1);
                if (!(component is TimedTurnOffer) && !(component is SelfDestruct) && !(component is Harmer) && !(component is Flare))
                {
                    int componentID = component.ComponentID;
                    componentFromTypeID.AddAttachmentToThis(component);
                    component.ForceComponentID(componentID);
                }
            }
            componentFromTypeID.ForceComponentID(id);
            return componentFromTypeID;
        }
    }
}
