using Colin.Gimbal;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace GimbalServerPro
{
	internal class botfix
	{
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		public static void AwardMoneyKill(Player killer, Player victim)
		{
			Console.WriteLine("Award Money Kill!");
			if (killer.PrimaryVehicle != null)
			{
				float num = 1f;
				if (victim.IsBot&& !victim.PrimaryVehicle.IsMothership)
				{
					num = 0.2f;
				}
				int num2 = GameLogicHandler.CalculateWinAmount(killer.VehicleProgress, victim.VehicleProgress, num);
				MethodInfo method = killer.GetType().GetMethod("WinMoney", BindingFlags.Instance | BindingFlags.NonPublic);
				method.Invoke(killer, new object[]
				{
					num2
				});
				if (!victim.IsBot)
				{
					MethodInfo method2 = killer.GetType().GetMethod("WinVehicle", BindingFlags.Instance | BindingFlags.NonPublic);
					method2.Invoke(killer, new object[]
					{
						victim.PrimaryVehicle.VehicleGuid
					});
				}
			}
		}
	}
}
