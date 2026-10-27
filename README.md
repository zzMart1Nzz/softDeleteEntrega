# Kiroldegia — Windows Forms (.NET Framework 4.7.2)

Ireki `softDeleteEntrega.slnx` Visual Studion eta exekutatu proiektua.
Interfazea euskaraz dago.

Taldekideen SELECT, INSERT eta DELETE APIak konektatzeko, irakurri [TALDEKIDEENTZAT.md](TALDEKIDEENTZAT.md).

## Git biltegiak

Biltegi hau C# interfazearena da: [softDeleteEntrega](https://github.com/zzMart1Nzz/softDeleteEntrega).
APIa beste biltegi batean dago, eta haren aldaketak `softDelete` adarrean egiten dira.

Tokiko karpetak honela antolatuta daude; bakoitzak bere `.git` karpeta dauka:

```text
entregaErrserba/
├── Api/                 ← APIaren Git biltegia
└── softDeleteEntrega/   ← interfazearen Git biltegia
```

Ez sortu Git biltegirik `entregaErrserba` karpeta nagusian, bi proiektuak nahas ez daitezen.
GitHub Desktop-en interfazea gehitzeko, erabili **File > Add local repository** eta hautatu
`entregaErrserba/softDeleteEntrega` karpeta, soluzioaren fitxategia duen karpeta bera.
`.vs`, `bin` eta `obj` karpetak `.gitignore` bidez baztertuta daude.

## Pantailak

- Login: **Bazkidearen ID-a** eremua eta **Sartu** botoia, pasahitzik gabe.
- Hasiera: **Erreserbak** eta **Pistak** botoi handiak.
- Erreserbak: taula, **Erreserba gehitu** eta **Berritu** botoiak. Ez dago erreserba-sistemarik edo sarrera-inprimakirik inplementatuta.
- Pistak: **Pistak** eta **Pista desgaituak** botoi handiak.
- Pista erabilgarrien taula: **Ezabatu** azken zutabean, soft delete egiteko.
- Pista desgaituen taula: **Berreskuratu** eta **Ezabatu** azken bi zutabeetan. Bigarren ekintza behin betiko DELETE funtziorako prestatuta dago.

Saioa hasteko, aplikazioak API bidez egiaztatzen du sartutako ID-a `kiroldegia.bazkidea` taulan dagoela. Bazkidea aurkituz gero hasierako menua irekitzen da, eta goiburuan ID-a, izena eta abizena agertzen dira. ID-a ez badago, ez da saiorik hasten. Hutsik dauden balioak, zenbaki negatiboak, zero, hamartarrak eta testua baztertzen dira.

**Interfazea aurreikusi (saioa hasi gabe)** estekarekin pantailak azter daitezke APIa edo datu-basea prest ez dagoenean ere. Aurreikuspenak ez du saiorik hasten eta datuak aldatzeko ekintzak blokeatuta daude. Ez dago erabiltzaile faltsurik edo adibideko daturik.

## Inplementatuta dagoena

`Services/ApiZerbitzua.cs` fitxategiak API deiak kudeatzen ditu:

| Ekintza | API deia | Emaitza |
| --- | --- | --- |
| Login | `POST api/Bazkidea/login`, gorputza: `{"id":1}` | `bazkidea` taulako ID-a egiaztatu eta bazkidea itzuli |
| Soft delete | `PATCH api/Pista/{id}/desgaitu` | `desgaituta = 1` |
| Berreskuratu | `PATCH api/Pista/{id}/berreskuratu` | `desgaituta = 0` |

APIko `PistaController`-ean bi eragiketek transakzio bera erabiltzen dute: pista **eta hari lotutako erreserba guztiak** egoera berera eguneratzen dira. Lehendik zegoen soft delete-aren jokabideari jarraitzen dio. Erreserba baten aurreko egoera bereizita gordetzeko logikarik ez dago; berreskuratzeak pista horren erreserba guztiak aktibatzen ditu.

Login-a `BazkideaController`-ean dago, eta ID parametrizatua erabiltzen du: `SELECT id, izena, abizena FROM bazkidea WHERE id = @id`. Hau da saioa hasteko gehitutako kontsulta bakarra; zerrenden SELECT funtzioak ez dira inplementatu. ID baliogabea bada HTTP 400, bazkidea falta bada 404, eta datu-baseko konexioak huts eginez gero 500 itzultzen dira.

APIak arrakasta baieztatzen duenean bakarrik kentzen da errenkada uneko taulatik. Beste zerrenda irekitzean edo **Berritu** sakatzean, SELECT funtzioak berriro kargatuko ditu datuak, taldekideak konektatzen duenean.

`App.config`-eko `ApiBaseUrl` balioa `http://localhost:5289/` da. APIa exekutatzeko, `Api/ApiErreserba` karpetatik: `dotnet run --launch-profile http`. Datu-basearen konexioa APIaren konfigurazioan dago; WinForms aplikazioak ez du MySQLra zuzenean konektatzen.

## Taldekideek osatzeko

Kontratua `Services/ITaldearenZerbitzua.cs` fitxategian dago. Inplementatzeko lekuak hiru fitxategitan banatuta daude:
`TaldearenZerbitzua.Select.cs`, `TaldearenZerbitzua.Insert.cs` eta `TaldearenZerbitzua.Delete.cs`.
Oraingoz `NotImplementedException` botatzen dute; interfazeak erabilgarri ez dagoela azaltzen du.
`ApiZerbitzua` bezeroak zerrendak irakurtzeko, datuak bidaltzeko eta DELETE egiteko HTTP laguntzaileak ditu,
baina ez da taldekideen endpoint-ik deitzen haiek metodoak konektatu arte.

| Metodoa / lekua | Egin beharrekoa |
| --- | --- |
| `LortuErreserbakAsync` | Erreserben SELECT konektatu. |
| `LortuPistakAsync(false)` | `desgaituta = 0` duten pistak soilik ekarri. |
| `LortuPistakAsync(true)` | `desgaituta = 1` duten pistak soilik ekarri. |
| `GehituErreserbaAsync` | INSERT konektatu. `Program.cs`-en `form.ErreserbaGehitzekoEkintza` lotu etorkizuneko sarrera-inprimakiari. |
| `EzabatuPistaAsync` | Desgaitutako pistaren behin betiko DELETE konektatu. Taulako azken botoia jada metodo horretara lotuta dago. |

Zerrenden SELECT endpoint-ak eta iragazkiak, INSERT-a eta behin betiko DELETE-a prestatu gabe daude. SELECT konektatu arte zerrendek ez dute daturik erakusten; soft delete eta berreskuratzeko botoiek benetako errenkadak eta saioa behar dituzte.

## Kodearen antolaketa

- `Form1.cs`: pantailak, nabigazioa eta botoien gertaerak.
- `Ui/Itxura.cs`: koloreak, botoiak eta taulen itxura.
- `Models/`: taulen eta saioaren datuak.
- `Services/ApiZerbitzua.cs`: IDarekin sartzeko POST deia eta inplementatutako bi PATCH deiak.
- `Services/TaldearenZerbitzua.*.cs`: SELECT, INSERT eta DELETE arduradun bakoitzaren TODOak.
- `DTO/ErreserbaSarreraDto.cs`: APIari bidaltzeko data eta orduaren formatuak.

## Egiaztapena

WinForms eta API proiektuak ondo konpilatu dira. Interfazearen eta HTTP bezeroaren 50 egiaztapen gainditu dira:
ID balidazioa, saioa, nabigazioa, taulak, bi PATCH eragiketak, `dataList` irakurtzea,
ISO datak eta orduak, DELETE 204/JSON erantzunak eta etorkizuneko INSERT ekintzaren kontrola.

API exekutatuarekin login-aren beste sei HTTP egiaztapen egin dira: falta den ID-a, zero, ID negatiboa, testua, zenbaki hamartarra eta datu-baseko errorearen tratamendua.

Benetako saioa ere egiaztatu da: Windows Forms -> API -> MySQL, bazkidearen IDarekin.
Hasierako MySQL sarbide-errorea tokiko pasahitza zuzenduta konpondu da.
Bakoitzak bere `Api/ApiErreserba/appsettings.Local.json` fitxategia erabiltzen du; fitxategi hori Git-ek baztertzen du.
Ez da benetako pisten edo erreserben daturik aldatu login proba horretan.
