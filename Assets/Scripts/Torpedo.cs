using UnityEngine;
using Photon.Pun;

public class Torpedo : MonoBehaviourPun
{
    [SerializeField]
    private float speed;

    [SerializeField]
    private float maxTime;

    public int OwnerViewID => ownerViewID;
    private float timer;
    public int ownerViewID;
    private Collider torpedoCollider;

    void Start()
    {
        timer = 0f;
        torpedoCollider = GetComponent<Collider>();
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        timer += Time.deltaTime;
        if (timer >= maxTime)
        {
            photonView.RPC("DestroyTorpedo", RpcTarget.AllBuffered);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        //Damage to other players
        if (other.CompareTag("Player"))
        {
            PhotonView playerView = other.GetComponent<PhotonView>();

            //Todo: Fix this. It doesn't really work properly, only one player seems protected. Rudementarily fixed by just making it not spawn near the player.
            if (playerView.ViewID != ownerViewID)
            {
                //Send the information to take damage to the other player
                playerView.RPC("TakeDamage", RpcTarget.AllBuffered, 10);
                if (photonView.IsMine)
                {
                    photonView.RPC("DestroyTorpedo", RpcTarget.AllBuffered);
                }
            }
        }
    }


    [PunRPC]
    public void DestroyTorpedo()
    {
        // Check if the client calling this is the owner
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    // Set the owner of the torpedo
    public void SetOwner(int viewID)
    {
        ownerViewID = viewID;
    }
}
