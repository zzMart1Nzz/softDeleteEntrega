# Taldekideen APIak konektatzeko gida

Interfazea prest dago; zerrenden SELECT, erreserben INSERT eta behin betiko DELETE ez daude inplementatuta.
Fitxategi bakoitzean dauden `throw new NotImplementedException(...)` lerroek aukera horiek blokeatzen dituzte.
Ez da adibideko daturik erabiltzen.

## Lanaren banaketa

| Lana | Aldatu beharreko fitxategia | Prest dagoena |
| --- | --- | --- |
| SELECT | `softDeleteEntrega/Services/TaldearenZerbitzua.Select.cs` | Erreserben eta pisten taulak, berritzeko botoiak eta egoeraren parametroa |
| INSERT | `softDeleteEntrega/Services/TaldearenZerbitzua.Insert.cs` | Sarreraren DTOa eta gehitzeko botoiaren konexio-puntua |
| DELETE | `softDeleteEntrega/Services/TaldearenZerbitzua.Delete.cs` | Pista desgaituen azken zutabeko botoia |

Hiru fitxategiak `TaldearenZerbitzua` klasearen zatiak dira (`partial`).
Denek `Api` bezero bera erabiltzen dute. Ez sortu beste HTTP bezerorik botoi bakoitzerako.
Ez da beharrezkoa nabigazioa edo taulen marrazketa aldatzea.

## 1. APIa eta tokiko konexioa

APIaren biltegian kopiatu `appsettings.Local.example.json` fitxategia `appsettings.Local.json` izenarekin,
eta jarri zure MySQL erabiltzailea eta pasahitza. Fitxategi lokala Git-ek baztertzen du.
Abiarazi APIa: `dotnet run --launch-profile http`.

Interfazearen `softDeleteEntrega/App.config` fitxategiko `ApiBaseUrl` balioa APIaren helbide bera izan behar da:
`http://localhost:5289/`. Taldekideen controller-ak API horretan bateratu ondoren helbide bakarra nahikoa da.
HTTP laguntzaileei pasa **benetako bide erlatiboa**, adibidez `api/...`; ez pasa helbide osoa edo hasierako `/` karakterea.
Gida honetako `BENETAKO_...` izenak markatzaileak dira: ordezkatu zure controller-aren route-arekin.

## 2. SELECT konektatu

`LortuErreserbakAsync` metodoan ordezkatu `throw` lerroa:

```csharp
return Api.LortuZerrendaAsync<Erreserba>("BENETAKO_ERRESERBEN_BIDEA");
```

`LortuPistakAsync(bool desgaituta)` metodoan erabili parametroa zure endpoint-aren iragazkia aukeratzeko.
`false` jasotzean `desgaituta = 0` duten pistak bakarrik itzuli; `true` jasotzean `desgaituta = 1` dutenak.
Zure route-ak query parametroa, segmentua edo bi endpoint erabiltzen baditu, egokitu bidea metodo horretan.

Zerrendaren erantzunak APIko `ApiResponse<T>.Ok(List<T>, ...)` formatua behar du:

```json
{"status":"success","code":200,"type":"list","message":"OK","dataList":[]}
```

Zerrenda hutsak `[]` itzultzen du. Akats baten aurrean ez itzuli zerrenda huts faltsurik.
`Data` eremuak `yyyy-MM-dd` erabiltzen du eta `Ordua` eremuak `HH:mm:ss`.
Taulak automatikoki beteko dira metodoak konektatzean.

## 3. INSERT konektatu

`GehituErreserbaAsync` metodoan ordezkatu `throw` lerroa:

```csharp
return Api.SortuAsync<Erreserba>(
    "BENETAKO_INSERT_BIDEA",
    ErreserbaSarreraDto.Sortu(erreserba));
```

DTOak `Pista_id`, `Bazkidea_id`, `Data` eta `Ordua` bakarrik bidaltzen ditu.
Data eta ordua ISO testu bihurtzen ditu; ez du erreserba bat sortzen edo balidatzen bere kabuz.
INSERT erantzunak sortutako erreserba `data` eremuan eta `type: "element"` itzuli behar ditu.

Ez dago sarrera-inprimakirik eraikita. Zure inprimakia prest dagoenean, lotu `Program.cs`-eko TODOan.
Hau etorkizuneko `ErreserbaSarreraForm` inprimakiaren lotura-adibidea da:

```csharp
form.ErreserbaGehitzekoEkintza = async bazkidea =>
{
    using (var sarrera = new ErreserbaSarreraForm())
    {
        if (sarrera.ShowDialog(form) != DialogResult.OK) return false;
        Erreserba berria = sarrera.Erreserba;
        berria.Bazkidea_id = bazkidea.Id;
        await taldearenZerbitzua.GehituErreserbaAsync(berria);
        return true;
    }
};
```

`true` itzultzean interfazeak zerrenda berritzen du; `false` itzultzean ez du berritzen.
Salbuespena bada, errorea erakusten du. Saioa hasi gabeko aurreikuspenak ez du ekintza exekutatzen.
`ErreserbaSarreraForm` eta haren `Erreserba` propietatea INSERTaren arduradunak sortu beharrekoak dira.

## 4. DELETE konektatu

`EzabatuPistaAsync` metodoan ordezkatu `throw` lerroa:

```csharp
return Api.EzabatuAsync("BENETAKO_DELETE_BIDEA/" + pistaId);
```

APIak HTTP 204 edo `status: "success"` duen JSON erantzuna itzul dezake.
Errore bat jasotzen bada, errenkada ez da taulatik kentzen.
Aktiboen **Ezabatu** botoiak soft delete egiten jarraitzen du; metodo hau desgaitutakoen behin betiko ezabatzeari dagokio.

## 5. Partekatutako kodea

- `ApiZerbitzua.cs`: HTTP, JSON, zerrendak, denbora-muga eta APIaren erroreen tratamendua.
- `ITaldearenZerbitzua.cs`: interfazeak erabiltzen duen kontratua; gorde metodoen sinadurak.
- `Form1.cs`: pantailak eta ekintzen lotura; INSERTerako konexio-puntua jada prestatuta dago.
- `DTO/ErreserbaSarreraDto.cs`: sarreraren data eta orduaren formatua.

Login-a, soft delete-a eta berreskuratzea dagoeneko konektatuta daude.
Ez aldatu horien route-ak gainerako APIak konektatzeko.

## GitHub Desktop-en lana ikustea

Hautatu **softDeleteEntrega** interfazea aldatzean, eta **Api / softDelete** controller-ak aldatzean.
`Changes` fitxak uneko biltegiko commit gabeko aldaketak bakarrik erakusten ditu.
Commit egin ondoren aldaketak `History` fitxan ikusten dira; `No local changes` normala da.
`bin`, `obj`, `.vs` eta MySQLren tokiko konfigurazioa ez dira partekatzen.
