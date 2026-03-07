VAR firstArea = false

- Ja tady domu všemu vlááádnu. Všem naokolo, uplne všem. I tobě...
    *   Jak jsi vědel že jsem tady?
    *   Mluvíš na mě?
    *   Buď zticha!
    *   {firstArea} [Tobě znám!] -> to_end

- Řekli mi živý mrtvý chodíci děti...
    *   Kdo[?] ti to řekl?
    *   Jaký děti?

- Mám všechny peníze na světe.
    *   To není špatné...
    *   Odkud ste je nabral?

- Nekecám paní!
    *   Já vám věřím.
    *   Skutečne?
    *   Já nejsem paní

- Všechny peníze!
    *   To je skutečne báječné.
    *   A to je kolik?
    *   Jak to?

- Ty nevěříš, i když ti nelžu!
    *   Říkam že [věřím.]jo.
    *   Proč si myslíš?
- -> to_end

=== to_end ===

~ firstArea = true

- Ti přikazuju se vzát!
    *   A komu?
    *   Tobě?
    *   Leda tak ve snu.

- -> END