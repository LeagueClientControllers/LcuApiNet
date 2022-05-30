using Newtonsoft.Json;

namespace LcuApiNet.Model.ClientModels.ChampSelectModels;

public class SessionChatDetails
{
    [JsonProperty("chatRoomName")]
    public string ChatRoomName { get; set; }

    [JsonProperty("chatRoomPassword")]
    public string ChatRoomPassword { get; set; }
}