using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

/// @author Kaisa Koski
/// @version 1/2021
/// <summary>
/// Pieni ohjelma, jonka tein nopeasti omaksi avuksi lainatarjousten vertailuun, kun
/// kyseessä on annuiteettilaina.
/// 
/// Ohjelmalla voi viitekorkoa muuttamalla myös tutkia, kuinka suureksi
/// kuukausittainen lainanmaksuerä ja kokonaislaina kasvaa erisuuruisilla viitekoroilla.
/// </summary>
public class Pankkivertailu
{
    /// <summary>
    /// Lainatarjousten tulostaminen.
    /// </summary>
    public static void Main()
    {
        string pankki1 = Vertaa("Pankki 1, nollakorot", 180000, "12kk Euribor", 0.52, 25, 3.5, 0, 380);
        string pankki2 = Vertaa("Pankki 2, nollakorot", 180000,"12kk Euribor", 0.40, 25, 2.5, 11.9, 400);
        string pankki3 = Vertaa("Pankki 3, nollakorot", 180000, "12kk Euribor", 0.49, 25, 2.7 * 2, 7.425, 270);
        string pankki1IsoillaKoroilla = Vertaa("Pankki 1, 5% viitekorko", 180000, "12kk Euribor", 5.52, 25, 3.5, 8, 380);


        Console.WriteLine(pankki1);
        Console.WriteLine(pankki2);
        Console.WriteLine(pankki3);
        Console.WriteLine(pankki1IsoillaKoroilla);


    }
    /// <summary>
    /// Muodostaa merkkijonon, johon kerätty lainatarjouksen tiedot sillä korkotiedolla, mikä on annettu.
    /// Lopullinen summa voi muuttua viitekoron muuttuessa.
    /// </summary>
    /// <param name="pankki">Pankin nimi ja muut tunnisteet</param>
    /// <param name="lainasumma">Lainan määrä</param>
    /// <param name="viitekorko">Mitä viitekorkoa pankki on tälle lainalle tarjonnut, esim. 12kk Euribor tai 6 kk Euribor. Ei vaikuta laskelmiin.</param>
    /// <param name="kokonaiskorko"> Koko koron määrä, eli viitekorko+marginaali+mahdollinen korkoputki, esim 3.5% korko = 3.5, 
    ///                              viitekoron ollessa nollassa on siis pelkästään marginaali + mahdollinen korkoputki</param>
    /// <param name="lainaAika">Laina-aika vuosina</param>
    /// <param name="kkmaksu">Lainanhoitoon liittyvät maksut kuukaudessa, euroina, esim. 4.5e/kk = 4.5</param>
    /// <param name="pankkipalvelut">Muut kuin lainanhoitoon liittyvät pankkimaksut kuukaudessa, euroina, esim. 4.5e/kk = 4.5</param>
    /// <param name="nostokulut">Kertaluontoinen lainannostokulu euroina</param>
    /// <returns></returns>
    public static string Vertaa(string pankki, double lainasumma, string viitekorko, double kokonaiskorko, 
                                int lainaAika, double kkmaksu, double pankkipalvelut, double nostokulut)
    {
        double korko = kokonaiskorko / 100;
        double kkmaksutKaikki = Math.Round(kkmaksu * 12 * lainaAika);
        double palvelutKaikki = Math.Round(pankkipalvelut * 12 * lainaAika);
        double annuiteetti = Math.Round(Annuiteetti(lainasumma, korko, lainaAika));
        double kkEra = Math.Round(Annuiteetti(lainasumma, korko, lainaAika) / 12);
        double kokoSumma = Math.Round(Kokonaissumma(lainasumma, korko, lainaAika));
        double kokoKorko = Math.Round(KoronMaara(lainasumma, korko, lainaAika));
        double yhteisSumma = Math.Round(KoronMaara(lainasumma, korko, lainaAika) + nostokulut + (kkmaksu * 12 * lainaAika) + (pankkipalvelut * 12 * lainaAika));

        string pankkitiedot = pankki + "\n" +
                              "Lainasumma: " + lainasumma + "\n" +
                              "Viitekorko: " + viitekorko + "\n" +
                              "Korkoprosentti: " + korko * 100 + "% \n" +
                              "Laina-aika: " + lainaAika + " vuotta \n" +
                              "Lainannostokulut: " + nostokulut + "\n" +
                              "Lainanhoitokulut kuukaudessa/koko laina-ajalta: " + kkmaksu + " e/" + kkmaksutKaikki + " e \n" +
                              "Pankkipalvelut kuukaudessa/koko laina-ajalta: " + pankkipalvelut + " e/" + palvelutKaikki + " e \n" +
                              "Annuiteetti eli vuosittain maksettava tasaerä: " + annuiteetti + " e \n" +
                              "Lainaerä kuukaudessa: " + kkEra +" e \n"+
                              "Maksettava summa kokonaisuudessaan on " + kokoSumma + " e, josta koron osuus " + kokoKorko + " e \n" +
                              "Lainan kokonaiskustannukset: " + (kokoKorko + nostokulut + kkmaksutKaikki) + " e \n" +
                              "Pankkiasiakkuuden hinta koko laina-ajalta: " + (palvelutKaikki) + " e \n" +
                              "Kaikki kulut yhteensä laina-ajalta: " + yhteisSumma + " e \n";
        return pankkitiedot;
    }

    /// <summary>
    /// Laskee ja palauttaa annuiteetin eli lainan vuodessa maksettavan tasaerän.
    /// </summary>
    /// <param name="lainasumma">Lainattava summa</param>
    /// <param name="korko">Koron määrä, esim 5% = 0.05</param>
    /// <param name="lainaAika">Laina-aika vuosina</param>
    /// <returns>Vuodessa maksettava tasaerä</returns>
    public static double Annuiteetti(double lainasumma, double korko, int lainaAika)
    {
        double apuluku = Math.Pow(1 + korko, lainaAika);
        double annuiteetti = apuluku * korko / (apuluku - 1) * lainasumma;
        return annuiteetti;
    }

    /// <summary>
    /// Laskee takaisinmaksettavan summan kokonaisuudessaan eli lainan määrä + korkokulut 
    /// annetun korkoprosentin mukaisesti.
    /// <param name="lainasumma">Lainattava summa</param>
    /// <param name="korko">Koron määrä, esim 5% = 0.05</param>
    /// <param name="lainaAika">Laina-aika vuosina</param>
    /// <returns>Lainan takaisinmaksettava määrä yhteensä</returns>
    public static double Kokonaissumma(double lainasumma, double korko, int lainaAika)
    {
        return lainaAika * Annuiteetti(lainasumma, korko, lainaAika);
    }

    /// <summary>
    /// Laskee koron määrän koko laina-ajalta käyttäen annettua korkomäärää
    /// (todellisuudessa koron vaihtelu vaikuttaa lopulliseen summaan).
    /// </summary>
    /// <param name="lainasumma">Lainattava summa</param>
    /// <param name="korko">Koron määrä, esim 5% = 0.05</param>
    /// <param name="lainaAika">Laina-aika vuosina</param>
    /// <returns>Koron kokonaismäärä</returns>
    public static double KoronMaara(double lainasumma, double korko, int lainaAika)
    {
        return Kokonaissumma(lainasumma, korko, lainaAika) - lainasumma;
    }

}
