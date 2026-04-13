using UnityEngine;

public class ProcessBlockInTowerZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Block"))
        {
            if (other.GetComponent<Supply>() != null)
            {
                other.GetComponent<Supply>().Respawn();
            }
            else
            {
                Destroy(other.gameObject);
            }
        }
    }
}
