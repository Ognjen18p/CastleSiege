using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTokens : MonoBehaviour {
    [Header("Attack Tokens Inspector")]
    public int tokens;
    [SerializeField] private int maxTokens = 1;

    private void Start() {
        tokens = maxTokens;
    }
    public bool CheckTakeToken() {
        if (tokens > 0) {
            tokens--;
            StartCoroutine(RefreshToken());
            return true;
        }
        return false;
    }
    IEnumerator RefreshToken() {
        yield return new WaitForSeconds(3f);
        if (tokens < maxTokens)
            tokens++;
    }
}
