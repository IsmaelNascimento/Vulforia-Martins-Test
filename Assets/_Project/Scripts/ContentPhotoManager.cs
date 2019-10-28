using UnityEngine;
using UnityEngine.UI;

public class ContentPhotoManager : MonoBehaviour
{
    [Header("Product")]
    [SerializeField] private GameObject m_PanelAboutProduct;
    [SerializeField] private Button m_ButtonBuyProduct;
    [SerializeField] private Text m_TextAboutProduct;
    [SerializeField] private Image m_ImageCupom;

    private void Start()
    {
        //Screen.orientation = ScreenOrientation.Portrait;
    }

    public void ShowAboutProduct(Product product)
    {
        m_PanelAboutProduct.SetActive(true);
        m_TextAboutProduct.text = string.Format(
            "Nome: \n{0}\n\n" +
            "Peso: \n{1}\n\n" +
            "Validade: \n{2}\n\n" +
            "Sabor: \n{3}", 
            product.nameProduct, product.netWeight, product.shelfLife, product.flavor);
        m_ImageCupom.sprite = product.spriteCupom;

        m_ButtonBuyProduct.onClick.AddListener(() => SaveCupom(out product.getCupom));
        m_ButtonBuyProduct.onClick.AddListener(() => print("product.getCupom:: " + product.getCupom));
    }

    private void SaveCupom(out bool getCupom)
    {
        getCupom = true;
        m_PanelAboutProduct.SetActive(false);
    }

    public void DisableGameObject(GameObject go)
    {
        go.SetActive(false);
    }
}