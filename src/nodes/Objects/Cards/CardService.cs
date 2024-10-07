﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using HalfNibbleGame.Autoload;

namespace HalfNibbleGame.Objects.Cards;

public sealed class CardService : Node
{
    private readonly Dictionary<Slot, Card> slots = new();
    private Board board;
    
    public List<Slot> SlotsInUse => slots.Keys.ToList();

    public Card? CardInUse { get; private set; }

    public event Action<Dictionary<Slot, Card>> OnCardListUpdated = _ => { };

    public override void _Ready()
    {
        Global.Services.ProvideInScene(this);
        base._Ready();

        board = GetNode<Board>("../Board");
    }

    public async Task UseCardInSlot(Slot slot)
    {
        var possibleCard = slots[slot];
        if (possibleCard is null)
            throw new InvalidOperationException("Trying to use empty slot");

        try
        {
            CardInUse = possibleCard;

            if (CardInUse is CardWithTarget cardWithTarget)
            {
                // TODO: Pick a target properly
                cardWithTarget.TargetTile = board.Tiles.First(x => x.Piece is not null);
            }

            await possibleCard.Use(board);
        }
        finally
        {
            CardInUse = null;
            slots.Remove(slot);
            OnCardListUpdated(slots);
        }
    }

    public void AddCardToSlot(Card card, Slot slot)
    {
        slots[slot] = card;
        OnCardListUpdated(slots);
    }

    public enum Slot
    {
        One,
        Two,
        Three
    }
}
