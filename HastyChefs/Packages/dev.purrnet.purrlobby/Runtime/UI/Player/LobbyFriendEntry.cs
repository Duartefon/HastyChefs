using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PurrNet.UI;


namespace PurrNet.Lobby
{
    public class LobbyFriendEntry : MonoBehaviour
    {
        [SerializeField] private RectangleGraphic _avatarGraphic;
        [SerializeField] private TMP_Text _name;
        [SerializeField] private TMP_Text _status;

        private string _id;
        private Action<string> _onInvite;


        public bool hasAvatar { get; private set; }

        public void Init(string id, Action<string> onInvite)
        {
            _id = id;
            _onInvite = onInvite;
        }

        public void SetAvatar(Texture2D texture)
        {
            if (!texture || !_avatarGraphic)
                return;

            _avatarGraphic.texture = texture;
            hasAvatar = true;
        }

        public void Refresh(string displayName, string status)
        {
            _name.text = displayName;
            _status.text = status;
        }

        public void OnClick()
        {
            _onInvite?.Invoke(_id);
            //_inviteButton.interactable = false;
            //if (_inviteButtonText) _inviteButtonText.text = "Invited";
            //Invoke(nameof(ResetInvite), 10f);
        }

        // private void ResetInvite()
        // {
        //     _inviteButton.interactable = true;
        //     if (_inviteButtonText) _inviteButtonText.text = "Invite";
        // }

    }
}
