// See https://aka.ms/new-console-template for more information

using LcuApiNet;
using LcuApiNet.Core.Events;
using LcuApiNet.Model.ClientModels;
using LcuApiNet.Model.Enums;

LcuApi api = new LcuApi();
api.LeagueEvents.Subscribe(LeagueEventType.GameflowPhaseChanged);
api.LeagueEvents.Subscribe(LeagueEventType.SessionInfoChanged);

api.LeagueEvents.GameflowPhaseChanged += async (sender, eventType, phase) => {
    Console.WriteLine($"[event: GameflowPhaseChanged] GameflowPhase = {phase}");
    if (phase == GameflowPhase.Lobby) {
        // await api.Matchmaking.StartMatchmaking().ConfigureAwait(false);
        // await api.Matchmaking.StopMatchmaking().ConfigureAwait(false);
    }
    // if (phase == GameflowPhase.ReadyCheck) {
    //     await api.Matchmaking.DeclineMatchAsync().ConfigureAwait(false);
    // }
};

//eventService.EventFired("");

api.CustomEvents.SelectStageStarted += (_, args) => {
    Console.BackgroundColor = ConsoleColor.Cyan;
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine($"[event: SelectStageStarted] BanStagePlanned={args.BanStagePlanned}, UserCellId = {args.UserPosition}:");
    Console.WriteLine("Ally pick order:");
    args.AllyPickOrder.ForEach(member => Console.WriteLine($"{member.UserId} | {member.Role}"));
    Console.ResetColor();
};

api.CustomEvents.PlanningStageEnded += async (_, _) =>  {
    Console.WriteLine("[event: PlanningStageEnded]");
};

api.CustomEvents.ChampionPicked += (_, args) => {
    Console.BackgroundColor = ConsoleColor.Green;
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine($"[event: ChampionPicked] ChampionId = [{args.ChampionId}], PickPosition = [{args.PlayerSelectPosition}], IsAllyAction = [{args.IsAllyAction}]");
    Console.ResetColor();
};

api.CustomEvents.ChampionBanned += (_, args) => {
    Console.BackgroundColor = ConsoleColor.Red;
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine($"[event: ChampionBanned] ChampionId = [{args.ChampionId}], PickPosition = [{args.PlayerSelectPosition}]");
    Console.ResetColor();
};

api.CustomEvents.ChampionSkinChanged += (_, args) => {
    Console.BackgroundColor = ConsoleColor.Gray;
    Console.WriteLine($"[event: ChampionSkinChanged] ChampionId = {args.ChampionId}, SkinId = {args.SkinId}");

    Console.ResetColor();
};

api.CustomEvents.ActionRequested += async (_, args) => {
    Console.BackgroundColor = ConsoleColor.Magenta;
    Console.BackgroundColor = ConsoleColor.White;
    Console.WriteLine($"[event: ActionRequested] Team = {args.Team}, ActionType = {args.ActionType.Name}, Action order:");
    args.PlayerSelectPositions.ToList().ForEach(pos => Console.WriteLine($"{pos}"));
    Console.ResetColor();
};


api.Client.StateChanged += async (_, args) => {
    if (args.State) {
        Summoner summoner = await api.Summoner.GetLocalUserInfo().ConfigureAwait(false);
        Console.WriteLine($"Name = {summoner.Name}, Tag = {summoner.GameTag}, IconId = {summoner.IconId}");
    }
};

api.CustomEvents.ChampionHovered += (_, args) => {
    Console.BackgroundColor = ConsoleColor.White;
    Console.WriteLine($"[event: ChampionHovered] ChampionId = {args.ChampionId}, PlayerSelectPosition =  {args.PlayerSelectPosition}");
    Console.ResetColor();
};

api.CustomEvents.SelectStageEnded += (_, args) => {
    Console.BackgroundColor = ConsoleColor.White;
    Console.WriteLine($"[event: SelectStageEnded] SelectStageEnded");
    Console.ResetColor();
};

// api.CustomEvents.UserActionRequested += async (_, args) => {
//     if (args.ActionType == ActionType.Pick) {
//         await api.Pick.HoverChampion(args.ActionId, 103,  () => {
//             _ = Task.Run(async () => {
//                 await Task.Delay(100);
//                 await api.Pick.LockChampion(args.ActionId).ConfigureAwait(false);
//             });
//         }).ConfigureAwait(false);
//     }
//     Console.WriteLine($"[event: LocalPlayerActionRequested] ActionId = {args.ActionId}, ActionType = {args.ActionType}");
// };



api.CustomEvents.PlanningStageEnded += async (_, _) => {
    Console.WriteLine("[PlanningStageEnded]");
};


await api.InitAsync().ConfigureAwait(false);
Console.ReadKey();
