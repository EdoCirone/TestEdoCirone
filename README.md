# TestEdoCirone

README
Inventory System Test

Sistema inventario realizzato in Unity con:

raccolta oggetti dal mondo
visualizzazione a slot fissi
utilizzo item con effetti diversi
drag and drop tra slot
drop degli oggetti nel mondo
HUD aggiornato tramite eventi
animazione di apertura/chiusura inventario
warning UI per inventario pieno
Funzionalità implementate
Inventario
Inventario a capacità fissa.
Gli slot vuoti sono rappresentati con null, così la UI mantiene un mapping stabile tra indice logico e slot visivo.
Aggiunta item al primo slot libero disponibile.
Rimozione item mantenendo invariata la dimensione della lista.
Spostamento o scambio item tra slot.
Raccolta item
Gli oggetti nel mondo possono essere raccolti cliccandoli.
Per semplicità di prototipazione la raccolta usa OnMouseDown() su ItemPickerControl.
Se l’inventario è pieno, l’oggetto non viene raccolto e viene mostrato un warning UI.
Uso item

Gli item usano ScriptableObject per definire l’effetto applicato al player.
Sono stati implementati quattro effetti:

Heal
Damage
Speed
Strength
Drag and drop
Drag di item tra slot.
Swap automatico se il target è occupato.
Se il drag termina fuori dal pannello inventario, l’item viene droppato nel mondo.
La preview dell’icona durante il drag viene creata a runtime come nuova Image UI dedicata.
HUD
Barra salute
Testo statistiche player
Aggiornamento reattivo tramite eventi (OnHealthChanged, OnStatsChanged)
UI e animazioni
Apertura inventario con animazione del pannello.
Comparsa scalata del titolo.
Apparizione a cascata degli slot.
Chiusura animata dell’inventario.
Warning per inventario pieno su canvas dedicato con sorting superiore rispetto alla UI principale.
Controlli
Click sinistro su item nel mondo → raccoglie l’oggetto
Click su slot occupato → usa l’item
Tasto I → apre / chiude inventario
Drag su uno slot occupato
sopra un altro slot → move/swap
fuori dal pannello inventario → drop nel mondo
Click fuori dal warning panel → chiude il warning di inventario pieno
Restart button → ricarica la scena corrente

Nota: ExitGame() è utile solo in build, non in Play Mode nell’editor.

Architettura
Manager e logica
InventoryManager
Gestisce lo stato dell’inventario, gli slot, l’uso item, gli eventi UI e le operazioni di move/swap.
PlayerStats
Gestisce salute e statistiche modificabili dagli item.
GameManager
Contiene utility semplici di scena (RestartLevel, ExitGame).
Item e dati
ItemData
Dati base di un item: nome, icona, prefab, descrizione, amount, effect.
AbstractEffectData
Classe base ScriptableObject per gli effetti item.
HealEffectData, DamageEffectData, SpeedEffectData, StrengthEffectData
Implementazioni concrete degli effetti applicabili.
Interazione mondo
ItemPickerControl
Rappresenta un item nel mondo e fornisce il dato ItemData da raccogliere.
PlayerItemCollector
Tenta di aggiungere l’item all’inventario.
PlayerItemDropper
Gestisce il drop nel mondo tramite raycast da posizione schermo.
UI
InventoryUI
Genera dinamicamente gli slot, inizializza la UI e si sottoscrive agli eventi dell’inventario.
InventorySlotUI
Gestisce visualizzazione, click sullo slot, drag and drop e preview dell’icona trascinata.
HUDManager
Aggiorna salute e stats in UI.
ResponsiveGrid
Calcola automaticamente la dimensione quadrata delle celle in base allo spazio disponibile.
InventoryAnimation
Gestisce apertura e chiusura animata del pannello inventario.
Scelte implementative principali
Inventario non singleton

InventoryManager non è stato implementato come singleton, perché la logica è pensata per essere riutilizzabile da più attori se necessario.

Slot fissi con null

La lista interna dell’inventario mantiene sempre dimensione costante.
Gli slot vuoti sono rappresentati con null, così:

la UI può mappare direttamente gli indici
la griglia resta stabile
move/swap e refresh diventano più semplici
Effetti tramite ScriptableObject

Gli effetti degli item sono separati dai dati item e dalla logica dell’inventario.
Per aggiungere un nuovo effetto basta creare una nuova sottoclasse di AbstractEffectData.

Drag preview separata

La preview dell’icona durante il drag non viene più ottenuta duplicando direttamente la Image originale dello slot, ma creando una nuova Image dedicata a runtime.

Problemi incontrati durante il test e soluzioni adottate
1. Preview dell’icona durante il drag con scala errata

Problema:
La preview dell’icona trascinata aveva dimensioni incoerenti quando veniva duplicata direttamente la Image dello slot e reparentata sotto il Canvas.

Causa:
L’elemento UI duplicato ereditava proprietà di dimensione e trasformazione non adatte a una preview libera fuori dalla gerarchia originale dello slot.

Soluzione:
È stata creata una nuova Image a runtime dedicata esclusivamente alla preview del drag, copiando solo sprite e proprietà visive essenziali e assegnando una dimensione fissa al RectTransform.

2. Warning panel non visibile sopra tutta la UI

Problema:
Il warning di inventario pieno non risultava sempre sopra agli altri elementi UI.

Soluzione:
È stato spostato su un canvas dedicato (WarningPanels) con Sort Order superiore rispetto alla UI principale.

3. Chiusura del warning al click

Problema:
Gestire la chiusura del warning tramite controllo input nello script era fragile.

Soluzione:
È stato usato un CloseArea UI cliccabile dietro al popup, così il warning si chiude con un click fuori dal pannello.

4. HUD aggiornato in polling continuo

Problema:
L’HUD veniva aggiornato ogni frame, anche quando i valori non cambiavano.

Soluzione:
PlayerStats espone eventi (OnHealthChanged, OnStatsChanged) e HUDManager si sottoscrive a questi eventi per aggiornare la UI solo quando serve.

5. Sequenza animazioni inventario

Problema:
L’apertura dell’inventario risultava poco leggibile quando title e slot comparivano tutti insieme.

Soluzione:
L’animazione è stata organizzata con DOTween Sequence:

pannello
titolo
comparsa slot in cascata con overlap temporale controllato
Limiti attuali
La raccolta nel mondo usa OnMouseDown() come soluzione di prototipo. In un progetto più strutturato sarebbe meglio usare un sistema di interazione dedicato.
La UI di drag and drop usa una reference statica (_draggedItem) per tracciare lo slot attualmente trascinato. Va bene per questo test, ma andrebbe rifatta nel caso di più inventari UI contemporanei.
Il sistema non supporta stack di item.
Il sistema non distingue ancora tra tipi di item più complessi (equipaggiabili, stackabili, consumabili non distruttivi, ecc.).
Il drop nel mondo richiede una superficie colpibile da raycast e una Main Camera valida.
Setup scena richiesto

Per il corretto funzionamento servono:

InventoryManager con riferimento a PlayerStats
Player con:
PlayerStats
PlayerItemCollector
PlayerItemDropper
UIManager con:
HUDManager
InventoryUI
InventoryAnimation
InventorySlotGrid con ResponsiveGrid
WarningPanels su canvas dedicato
EventSystem
Main Camera
Collider sulle superfici su cui è possibile droppare oggetti
Prefab item assegnati nei rispettivi ItemData
Struttura asset e script
Script
Scripts/Managers
Scripts/Player
Scripts/UI
Scripts/Data / Scripts/ScriptableObjects se si vuole rifinire ulteriormente la nomenclatura
Dati
Data/Items
Data/Effects
Prefab
Prefabs/Items
Prefabs/UI
Possibili estensioni future
inventari multipli (player / chest / NPC)
item stackabili
tooltip item
filtro/ordinamento inventario
sistema input centralizzato
interazione mondo più robusta
feedback audio sulle azioni UI
animazione warning panel
Build testata

Sistema verificato in editor e in build Windows con:

raccolta item
warning inventario pieno
uso item
drag and drop tra slot
drop fuori pannello
aggiornamento HUD
apertura/chiusura inventario
restart scena
