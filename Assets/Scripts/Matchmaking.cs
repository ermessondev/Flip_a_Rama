using UnityEngine;
using Fusion;
using System.Threading.Tasks;

public enum GameMode : int {x2, x4}
public class Matchmaking : MonoBehaviour
{
    private NetworkRunner _runner;

    private void Awake()
    {
        _runner = GetComponent<NetworkRunner>();
    }

}
