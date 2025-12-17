using UnityEngine;


public class FriendRedDot : MonoBehaviour
{
    [SerializeField] private GameObject redDot;
    public UserData.Friends.FriendInfo.FriendState appearsOn;


    private void Start()
    {
        UserData.Local.friends.onFriendsUpdate += HandleRedDot;
    }

    private void OnDestroy()
    {
        UserData.Local.friends.onFriendsUpdate -= HandleRedDot;
    }

    private void OnEnable() =>
        HandleRedDot();


    public void HandleRedDot()
    {
        var values = UserData.Local.friends.friendList.Values;
        if (values.Count > 0)
        {
            foreach (var v in values)
            {
                if (v.state == appearsOn)
                { redDot.SetActive(true); return; }
            }
            redDot.SetActive(false);
        }
        else
            redDot.SetActive(false);
    }

    
}
