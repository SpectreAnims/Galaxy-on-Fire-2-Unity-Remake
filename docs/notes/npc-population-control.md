# NPC population control — implementation handoff

## Scope
This PR adds a per-system ambient NPC population multiplier for modders. It is intentionally separate from secondary-weapon work.

## Mod format
- `systems.json` supports optional numeric `population`, range 0..4.
- Default/omitted value is 1 (vanilla behaviour).
- 0 suppresses baseline ambient traffic; 0.5 halves it; 2 doubles it; 4 is the upper bound.
- The Mod Builder exposes the same value on both new systems and system overrides.

## Runtime behaviour
- The multiplier is applied in `TrafficPlan.Build` to the baseline counts for local patrols, jumpers, freighters and random raider groups.
- It is applied before the later mission/story/special encounter rules. Those rules remain independent, including explicit encounter minimums and spawns.
- The zero-population fallback that normally adds four locals is disabled only when the configured multiplier is zero.
- Multiplayer already elects one orbit authority to build NPC traffic; that authority's resulting ships are replicated through the existing NetOrbit/NetProxy path. No second population generator or new network protocol is introduced.

## Protected behaviour / deliberate exclusions
Do not broaden this into a blanket cap or cleanup pass over `Traffic.Ships`. Never despawn NPCs by current system count. Mission, story, freelance, pirate-event, pirate-base, siege, rift/Void, campaign and explicitly spawned NPCs must remain intact.
Station-attracted traffic is deferred until player-built stations exist.
Mod NPC ship types with their own explicit `npcships.json` spawn rules remain governed by those rules, not this ambient multiplier.

## Review and verification
- Check that the scaling point remains before the scripted overrides in `TrafficPlan.Build`.
- Check new-system and override parsing, including malformed and out-of-range values.
- Unity compile and EditMode tests still need to be run in the Unity project.
- Exercise 0, 0.5, 1, 2 and 4 in a new system and an override.
- Verify mission/story/pirate/siege/rift spawns remain present at population 0.
- Verify multiplayer clients see the authority's single population, including authority handover.
- Mod Builder JSON round-trip must preserve unknown system fields and omit `population` when at the default 1.

## Claude handoff
If further review is needed, start from the PR branch and compare it with the exact base commit. Please inspect the ambient-vs-scripted spawn boundaries before changing any counts; do not replace the explicit encounter logic with a global NPC cap.
