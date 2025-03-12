using CalamityMagicStorage.FakeRarities;
using MagicStorage.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMagicStorage.Items;

public class ShadowspecStorageUnitItem : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 1;
    }
    public override void SetDefaults()
    {
        Item.width = 32;
        Item.height = 32;
        Item.maxStack = 99;
        Item.useTurn = true;
        Item.autoReuse = true;
        Item.useAnimation = 15;
        Item.useTime = 10;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.consumable = true;
        Item.rare = ModContent.RarityType<FakeHotPink>();
        Item.createTile = ModContent.TileType<Components.CalamityStorageUnit>();
        Item.placeStyle = 3;
    }

    public override void AddRecipes()
    {
        var recipe = CreateRecipe();
        recipe.AddIngredient<StorageComponent>();
        recipe.AddRecipeGroup("MagicStorage:AnyDiamond", 1);
        recipe.AddTile(TileID.WorkBenches);
        recipe.Register();
    }
}