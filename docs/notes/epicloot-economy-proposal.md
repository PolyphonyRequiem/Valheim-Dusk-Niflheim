## Issues:
- Too many shards without enough uses.
- Dust feels kinda redundant.
- Disenchanting feels a bit weak given the current loot economy (low drop rates of items).
- Re-augs need to be revisited.
- Runestones feel weird.
- Enchanting access feels too late.

## Proposals:
- Make the enchanter into a workbench upgrade, uses 50 shards.
- Make enchanting weapons cost Shards + Essence + Runestone
- Make enchanting armor cost Shards + Essence + Runestone
- Trophies no longer sacrifice into shards.  Only Dust, Essence, Reagents, or in rare cases like bosses, into runestones.
- Epic loot drop rates reduced to give a flat 50% chance base-rate of no-drop.
- Runestone crafting is now more straight forward.  Shards + Dust + tier-locked material.  (Finewood, ElderBark, Crystal, BlackMetalScrap)
- Runestone up-tiering will now be 5 x Lower Tier Runestone + the tier-locked material. (ElderBark, Crystal, BlackMetalScrap)
- Legendary Enchanting will require a unique item per category, all of which will be Mountains biome or later materials.  Examples:
  - Bow - 1 x Needle
  - 1H Weapon - 1 x Wolf Fang
  - TwoHandedWeapon - 1 x Crystal
  - Tool - 1 x BlackMetalScrap
  - Torch - 1 x FreezeGland
  - Shield - 1 x Obsidian
  - Helmet - 1 x Drake Egg
  - Chest - 2 x Flax
  - Legs - 1 x SilverOre
  - Shoulder - 1 x Lox Hide
  - Utility - 1 x CloudBerry

## Discussions:
- Unclear what to do with re-augmentation, but using Dust/Shards/Tokens seems logical.

## For Randy:
 - Enchanter/Augmenter materials and crafting station bindings configurable.
 - Augmentation caps (max number of augmentation slots) by configuration.

Also features I'm looking into:
 - Augmentation base costs with attempt accumulation, such as increasing shard cost each attempt - (haven't done this yet, but I assume it could be stored easily enough in the item data using extended item data framework, no?)
 - A "cleansing" process to dis-enchant an item and return it to being mundane with the intent of enchanting it again.  Probably would be an augmenter feature?  Would use configurable costs by category and rarity just like disenchant yields do right now.
