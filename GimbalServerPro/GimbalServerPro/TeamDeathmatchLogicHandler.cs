using Colin.Gimbal;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.Reflection;

namespace GimbalServerPro
{
    internal class TeamDeathmatchLogicHandler : GameLogicHandler
    {
        public override GameTimerStyle GameTimerStyle
        {
            get
            {
                return GameTimerStyle.MonolithicGame;
            }
        }

        public TeamDeathmatchLogicHandler(GimbalGameInstance GameInstance) : base(GameInstance)
        {
        }

        public override void HandleGameConditions(GameTime gameTime)
        {
            MethodInfo methodInfo = base.GameInstance.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).First((MethodInfo mi) => mi.Name == "Broadcast" && mi.GetParameters().Length == 1);
            MethodInfo method = base.GameInstance.GetType().GetMethod("StartNextGameWithLoadingScreen", BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo method2 = base.GameInstance.World.GetType().GetMethod("PushNotification", BindingFlags.Instance | BindingFlags.NonPublic);
            if (base.GameState == GameState.EndOfGame)
            {
                if (this.mNeedScoreboardInAFewSeconds && gameTime.TotalGameTime.TotalSeconds > this.mGameStateTimestamp.TotalSeconds + 8.0)
                {
                    if (!(GimbalGameManager.ScreenManager.TopScreen is ScoreboardScreen))
                    {
                        base.GameInstance.ToggleScoreboard_Callback(null, new EventArgs());
                        methodInfo.Invoke(base.GameInstance, new object[]
                        {
                            new ShowScoreboardMessage()
                        });
                        base.GameInstance.PrintScores();
                    }
                    this.mNeedScoreboardInAFewSeconds = false;
                }
                if (gameTime.TotalGameTime.TotalSeconds - this.mGameStateTimestamp.TotalSeconds > 25.0)
                {
                    method.Invoke(base.GameInstance, new object[0]);
                }
            }
            if (base.GameState == GameState.Combat && base.GameTimeLeftInSeconds <= 0f)
            {
                base.GameState = GameState.EndOfGame;
                this.mGameStateTimestamp = gameTime.TotalGameTime;
                Team team = null;
                if (base.GameInstance.World.TeamA.RoundFrags > base.GameInstance.World.TeamB.RoundFrags)
                {
                    team = base.GameInstance.World.TeamA;
                }
                if (base.GameInstance.World.TeamA.RoundFrags < base.GameInstance.World.TeamB.RoundFrags)
                {
                    team = base.GameInstance.World.TeamB;
                }
                if (team != null)
                {
                    GameEndData gameEndData = new GameEndData(team);
                    base.BroadcastGameEnd(gameEndData);
                    method2.Invoke(base.GameInstance.World, new object[]
                    {
                        new Notification(team.DisplayName + " wins the game!", 6f)
                    });
                    GimbalGameManager.GimbalInstance.ChatSay(new ChatMessage(null, team.DisplayName + " wins the game!", MessageType.GameChat));
                    base.AwardMoneyForWinGame(team);
                }
                else
                {
                    base.BroadcastGameEnd(new GameEndData
                    {
                        Draw = true
                    });
                    method2.Invoke(base.GameInstance.World, new object[]
                    {
                        new Notification("Game Draw!", 6f)
                    });
                    base.GameInstance.ChatSay(new ChatMessage(null, "Game draw.", MessageType.GameChat));
                }
                GimbalGameManager.GimbalInstance.ChatSay(new ChatMessage(null, "Game has ended.", MessageType.GameChat));
                this.mNeedScoreboardInAFewSeconds = true;
            }
        }

        public override void SetupTeams()
        {
            base.GameInstance.World.SetupTeams();
        }
    }
}
