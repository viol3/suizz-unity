using Ali.Helper;
using Sui.Accounts;
using Sui.Cryptography;
using Sui.Rpc;
using Sui.Rpc.Client;
using Sui.Utilities;
using UnityEngine;

public class SuiManager : LocalSingleton<SuiManager>
{
    Account _account;

    void Start()
    {
        _account = new Account("0x4cd576b79d2f5e522b1408b9b63cbd75c31b3fdb9e953ee8274c7c19b16783bd");
        EventBus.OnSponsoredTxArrived.AddListener(OnSponsoredTxArrived);
    }

    private void OnDestroy()
    {
        EventBus.OnSponsoredTxArrived.RemoveListener(OnSponsoredTxArrived);
    }

    void OnSponsoredTxArrived(string bytes, string digest)
    {
        byte[] bytes2 = System.Convert.FromBase64String(bytes);
        SignatureBase signature = _account.Sign(bytes2);
        WebSocketManager.Instance.SendSponsoredSignature(signature.ToBase64(), digest);
    }



}

public class SignatureRequest
{
    public string signature;
}
