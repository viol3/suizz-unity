using Sui.Accounts;
using Sui.Cryptography.Ed25519;
using Sui.Rpc;
using Sui.Rpc.Client;
using System;
using System.Security.Cryptography;
using UnityEngine;

public class SuiManager : MonoBehaviour
{
    SuiClient _client;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _client = new SuiClient(Constants.TestnetConnection);

        Debug.Log(GenerateEphemeralPublicKey("naber"));
        
    }

    public static string GenerateEphemeralPublicKey(string seed)
    {
        // 1. Seed’i byte array’e çevir
        byte[] seedBytes = System.Text.Encoding.UTF8.GetBytes(seed);

        // 2. SHA256 ile 32 byte hash üret
        using (SHA256 sha = SHA256.Create())
        {
            byte[] hash = sha.ComputeHash(seedBytes); // 32 byte
            // 3. Hex’e çevir ve 0x prefix ekle
            string pubHex = "0x" + BitConverter.ToString(hash).Replace("-", "").ToLower();
            return pubHex;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
