namespace PriceCheck
{
    /// <summary>
    /// Internal action code enum for use with context menus.
    /// </summary>
    public enum InternalAction : byte
    {
        #pragma warning disable 1591
        RetrieveFromRetainer = 19,
        HaveRetainerSellItems = 24,
        Equip = 25,
        Use = 26,
        Split = 28,
        Discard = 29,
        Unequip = 30,
        Repair = 31,
        ExtractMateria = 32,
        Meld = 33,
        Dye = 35,
        CastGlamour = 36,
        PlaceInArmouryChest = 37,
        Sort = 40,
        Link = 41,
        Desynthesis = 43,
        ItemComparison = 44,
        SearchForItem = 45,
        TryOn = 46,
        LowerQuality = 48,
        SecondTier = 49,
        RetrieveMateria = 55,
        SearchRecipesUsingThisMaterial = 64,
        #pragma warning restore 1591
    }
}
