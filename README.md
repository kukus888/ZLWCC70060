Prerekvizity
Je třeba mít nainstalovaný .NET 5.0. Lze ho stáhnout z oficiálních stránek (doporučuji zkusit spustit bez něj. Verze .NET 6.0 by měla fungovat také):
https://dotnet.microsoft.com/en-us/download/dotnet
Je třeba mít nainstalovaný SAP (konktaktovat helpdesk).

Instalace
Lze fungovat bez instalování, stačí rozkliknout SKUassignment.exe
Pokud chcete instalovat => setup.exe.

Jak to funguje?
Do souboru SKUS.txt vložíš produktové kódy a SKU, oddělené čárkou. Seznam jako takový se dá udělat třeba v excelu (funkce TEXTJOIN). Doporučuju mít seznam seřazený podle SKU (při ověřování je někdy nemusí najít, ale zatím to nebyla nijak velká katastrofa).
Zapni program. Pokud nemáš otevřený SAP, měl by tě k tomu vyzvat. SAP ukáže okno "A script is trying to access SAP GUI". Na to klikni OK.
Přihlas se do CEP. Bacha, program vás pustí dál i bez přihlášení (ale spadne pokud se ho budete snažit spustit bez přihlášení).
Program má 3 hlavní funkce:
Display models - můžete procházet modely, měnit vlastnosti atp... Po ověření se modely zabarví do zelena.
Write to table - Zapíše všechno do sapu. V průběhu může házet chybové hlášky, od toho je pak ta kontrola.
Check table - projde zapsané modely, a porovná je s tabulkou, co měl zapsat. Na konci vychrlí kolik jich je a není zapsaných. V display models jsou zeleně označené zapsané modely.

To by mělo být všechno. Otázky na j.sykora@samsung.com