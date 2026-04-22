using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObjects/ItemData", order = 1)]
public class ItemData : ScriptableObject
{
    [SerializeField] private string _itemName;
    [SerializeField] private Sprite _itemIcon;
    [SerializeField] private GameObject _prefab;
    [TextArea][SerializeField] private string _description;
    [SerializeField] private int _amount;
    [SerializeField] private AbstractEffectData _effect; //L'effetto viene deciso da uno scripble object a parte 

    public string ItemName => _itemName;
    public Sprite ItemIcon => _itemIcon;
    public GameObject Prefab => _prefab;
    public string Description => _description;
    public int Amount => _amount;
    public AbstractEffectData Effect => _effect;


}
