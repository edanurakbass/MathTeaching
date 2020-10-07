using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class gameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject karePrefab;

    [SerializeField]
    private Transform karelerPaneli;

    private GameObject[] karelerDizisi = new GameObject[25];

    [SerializeField]
    private Transform soruPaneli;

    [SerializeField]
    private Text soruText;

    [SerializeField]
    private Sprite[] kareSprites;

    [SerializeField]
    private GameObject sonucPaneli;

    [SerializeField]
    AudioSource audioSource;

    public AudioClip butonSesi;

    

    List<int> bolumDegerleriListesi = new List<int>();
    int bolunenSayi, bolenSayi;
    int kacinciSoru;
    int dogruSonuc;

    int butonDegeri;

    bool butonaBasildiMi;

    int kalanHak;

    string sorununZorlukDerecesi;

    kalanHakManager kalanHakManager;
    PuanManager puanManager;
    GameObject gecerliKare;
    private void Awake()
    {
        kalanHak = 3;

        audioSource = GetComponent<AudioSource>();

        sonucPaneli.GetComponent<RectTransform>().localScale = Vector3.zero;

        kalanHakManager = Object.FindObjectOfType<kalanHakManager>();
        puanManager = Object.FindObjectOfType<PuanManager>();

        kalanHakManager.KalanHaklariKontrolEt(kalanHak);
    }
    void Start()
    {
        butonaBasildiMi = false;
        soruPaneli.GetComponent<RectTransform>().localScale = Vector3.zero;

        kareleriOlustur();
    }

    public void kareleriOlustur()
    {
        for (int i = 0; i < 25; i++)
        {
            GameObject kare = Instantiate(karePrefab, karelerPaneli);
            kare.transform.GetChild(1).GetComponent<Image>().sprite = kareSprites[Random.Range(0, kareSprites.Length)];
            kare.transform.GetComponent<Button>().onClick.AddListener(() => ButonaBasildi());

            karelerDizisi[i] = kare;
        }
        bolumDegerleriTexteYazdir();
        StartCoroutine(DoFadeRoutine());
        Invoke("SoruPaneliAc", 2f);

        void ButonaBasildi()
        {
            if (butonaBasildiMi)
            {
                audioSource.PlayOneShot(butonSesi);
                butonDegeri = int.Parse(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.transform.GetChild(0).GetComponent<Text>().text);
                gecerliKare = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
                sonucuKontrolEt();
            }

        }

        void sonucuKontrolEt()
        {
            if (butonDegeri == dogruSonuc)
            {
                gecerliKare.transform.GetChild(1).GetComponent<Image>().enabled = true;
                gecerliKare.transform.GetChild(0).GetComponent<Text>().text = "";
                gecerliKare.transform.GetComponent<Button>().interactable = false;

                puanManager.PuaniArtir(sorununZorlukDerecesi);
                bolumDegerleriListesi.RemoveAt(kacinciSoru);

                if (bolumDegerleriListesi.Count > 0)
                {
                    SoruPaneliAc();
                }
                else
                {
                    OyunBitti();
                }
            }

            else
            {
                kalanHak--;
                kalanHakManager.KalanHaklariKontrolEt(kalanHak);

            }

            if (kalanHak<=0)
            {
                OyunBitti();
            }
        }

        void OyunBitti()
        {
            butonaBasildiMi = false;
            sonucPaneli.GetComponent<RectTransform>().DOScale(1, 0.3f).SetEase(Ease.OutBack);

        }
    }
    IEnumerator DoFadeRoutine()
    {
        foreach (var kare in karelerDizisi)
        {
            kare.GetComponent<CanvasGroup>().DOFade(1, .2f);
            yield return new WaitForSeconds(0.1f);
        }
    }


    private void bolumDegerleriTexteYazdir()
    {
        foreach (var kare in karelerDizisi)
        {
            int rastgeleDeger = Random.Range(1, 13);
            bolumDegerleriListesi.Add(rastgeleDeger);

            kare.transform.GetChild(0).GetComponent<Text>().text = rastgeleDeger.ToString();
        }

        
    }

    void SoruPaneliAc()
    {
        SoruyuSor();
        butonaBasildiMi = true;
        soruPaneli.GetComponent<RectTransform>().DOScale(1, 0.3f).SetEase(Ease.OutBack);
            
    }

    void SoruyuSor()
    {
        bolenSayi = Random.Range(2, 11);
        kacinciSoru = Random.Range(0, bolumDegerleriListesi.Count);
        dogruSonuc = bolumDegerleriListesi[kacinciSoru];
        bolunenSayi = bolenSayi * dogruSonuc;

        if (bolunenSayi<=40)
        {
            sorununZorlukDerecesi = "kolay";
        }
        else if (bolunenSayi>40 && bolunenSayi<=80)
        {
            sorununZorlukDerecesi = "orta";
        }
        else
        {
            sorununZorlukDerecesi = "zor";
        }
        soruText.text = bolunenSayi.ToString() + ":" + bolenSayi.ToString();
    }
}
