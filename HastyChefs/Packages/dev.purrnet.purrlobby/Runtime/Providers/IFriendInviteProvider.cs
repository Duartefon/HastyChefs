using System.Collections.Generic;
using UnityEngine;

namespace PurrNet.Lobby
{
    public struct FriendInfo
    {
        public string id;
        public string name;
        public bool online;
        public bool inThisGame;
    }

    public interface IFriendInviteProvider
    {
        List<FriendInfo> GetFriends(bool onlineOnly);
        bool InviteFriend(string lobbyId, string friendId);
        Texture2D GetAvatar(string friendId);
    }
}