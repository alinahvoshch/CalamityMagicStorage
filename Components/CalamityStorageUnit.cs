using System.Runtime.CompilerServices;
using MagicStorage.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using MagicStorage.Items;
using MagicStorage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ObjectData;
using CalamityMagicStorage;
using CalamityMagicStorage.Items;

namespace CalamityMagicStorage.Components
{

    public class CalamityStorageUnit : MagicStorage.Components.StorageUnit
    {
        public static int storageType = 0;
        public override int ItemType(int frameX, int frameY)
        {
            int style = frameY / 36;
            if (style == 1)
            {
                storageType = 1;
                return ModContent.ItemType<CosmiliteStorageUnitItem>();
            }
            else if (style == 2)
            {
                storageType = 2;
                return ModContent.ItemType<AuricStorageUnitItem>();
            }
            else if (style == 3)
            {
                storageType = 3;
                return ModContent.ItemType<ShadowspecStorageUnitItem>();
            }
            else
            {
                return ModContent.ItemType<UelibloomStorageUnitItem>(); // Default case
            }
        }
        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (Main.tile[i, j].TileFrameX % 36 == 18)
                i--;
            if (Main.tile[i, j].TileFrameY % 36 == 6)
                j--;

            if (TileEntity.ByPosition.ContainsKey(new Point16(i, j)) && Main.tile[i, j].TileFrameX / 36 % 3 != 0)
                fail = true;
        }
        public override bool CanExplode(int i, int j)
        {
            bool fail = false, discard = false, discard2 = false;

            KillTile(i, j, ref fail, ref discard, ref discard2);

            return !fail;
        }
        public override bool RightClick(int i, int j)
        {
            if (Main.tile[i, j].TileFrameX % 36 == 18)
                i--;
            if (Main.tile[i, j].TileFrameY % 36 == 6)
                j--;
            if (TryUpgrade(i, j))
                return true;

            if (!TileEntity.ByPosition.TryGetValue(new Point16(i, j), out var te) || te is not TECalamityStorageUnit calamityStorageUnit)
                return false;

            Main.LocalPlayer.tileInteractionHappened = true;
            string activeString = calamityStorageUnit.Inactive ? Language.GetTextValue("Mods.MagicStorage.Inactive") : Language.GetTextValue("Mods.MagicStorage.Active");
            string fullnessString = Language.GetTextValue("Mods.MagicStorage.Capacity", calamityStorageUnit.NumItems, calamityStorageUnit.Capacity);
            Main.NewText(activeString + ", " + fullnessString);
            return base.RightClick(i, j);
        }
        private static bool TryUpgrade(int i, int j)
        {
            Player player = Main.LocalPlayer;
            Item item = player.HeldItem;
            int style = Main.tile[i, j].TileFrameY / 36;
            bool success = false;
            if (style == 1 && item.type == ModContent.ItemType<UpgradeCosmilite>())
            {
                SetStyle(i, j, 2);
                success = true;
            }
            else if (style == 2 && item.type == ModContent.ItemType<UpgradeAuric>())
            {
                SetStyle(i, j, 3);
                success = true;
            }
            else if (style == 3 && item.type == ModContent.ItemType<UpgradeShadowspec>())
            {
                SetStyle(i, j, 4);
                success = true;
            }
            if (success)
            {
                if (!TileEntity.ByPosition.TryGetValue(new Point16(i, j), out var te) || te is not TECalamityStorageUnit calamityStorageUnit)
                    return false;

                calamityStorageUnit.UpdateTileFrame();
                NetMessage.SendTileSquare(Main.myPlayer, i, j, 2, 2);
                TEStorageHeart heart = calamityStorageUnit.GetHeart();
                if (heart is not null)
                {
                    if (Main.netMode == NetmodeID.SinglePlayer)
                        heart.ResetCompactStage();
                    else if (Main.netMode == NetmodeID.MultiplayerClient)
                        NetHelper.SendResetCompactStage(heart.Position);
                }

                item.stack--;
                if (item.stack <= 0)
                    item.SetDefaults();
                if (player.selectedItem == 58)
                    Main.mouseItem = item.Clone();

                SoundEngine.PlaySound(SoundID.MaxMana, calamityStorageUnit.Position.ToWorldCoordinates());
                Dust.NewDustPerfect(calamityStorageUnit.Position.ToWorldCoordinates(), DustID.PureSpray, Vector2.Zero, Scale: 2, newColor: Color.Green);

                return true;
            }

            return false;
        }
        private static void SetStyle(int i, int j, int style)
        {
            Main.tile[i, j].TileFrameY = (short)(36 * style);
            Main.tile[i + 1, j].TileFrameY = (short)(36 * style);
            Main.tile[i, j + 1].TileFrameY = (short)(36 * style + 18);
            Main.tile[i + 1, j + 1].TileFrameY = (short)(36 * style + 18);
        }
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Vector2 drawPos = zero + 16f * new Vector2(i, j) - Main.screenPosition;
            Rectangle frame = new(tile.TileFrameX, tile.TileFrameY, 16, 16);
            Color lightColor = Lighting.GetColor(i, j, Color.White);
            Color color = Color.Lerp(Color.White, lightColor, 0.5f);
            spriteBatch.Draw(Mod.Assets.Request<Texture2D>("Components/CalamityStorageUnit_Glow").Value, drawPos, frame, color);
        }
    }
}