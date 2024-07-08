﻿using System;
using System.Runtime.InteropServices;
using Dalamud.Game;

namespace XivCommon.Functions;

internal class UiAlloc {
    private static class Signatures {
        internal const string GameAlloc = "E8 ?? ?? ?? ?? 48 8D 77 28";
        internal const string GameFree = "E8 ?? ?? ?? ?? 4C 89 7B 60";
        internal const string GetGameAllocator = "E8 ?? ?? ?? ?? 8B 75 08";
    }

    private delegate IntPtr GameAllocDelegate(ulong size, IntPtr unk, IntPtr allocator, IntPtr alignment);

    private readonly GameAllocDelegate? _gameAlloc;

    private delegate IntPtr GameFreeDelegate(IntPtr a1);

    private readonly GameFreeDelegate? _gameFree;

    private delegate IntPtr GetGameAllocatorDelegate();

    private readonly GetGameAllocatorDelegate? _getGameAllocator;

    internal UiAlloc(ISigScanner scanner) {
        if (scanner.TryScanText(Signatures.GameAlloc, out var gameAllocPtr, "UiAlloc (GameAlloc)")) {
            this._gameAlloc = Marshal.GetDelegateForFunctionPointer<GameAllocDelegate>(gameAllocPtr);
        } else {
            return;
        }

        if (scanner.TryScanText(Signatures.GameFree, out var gameFreePtr, "UiAlloc (GameFree)")) {
            this._gameFree = Marshal.GetDelegateForFunctionPointer<GameFreeDelegate>(gameFreePtr);
        } else {
            return;
        }

        if (scanner.TryScanText(Signatures.GetGameAllocator, out var getAllocatorPtr, "UiAlloc (GetGameAllocator)")) {
            this._getGameAllocator = Marshal.GetDelegateForFunctionPointer<GetGameAllocatorDelegate>(getAllocatorPtr);
        }
    }

    internal IntPtr Alloc(ulong size) {
        if (this._getGameAllocator == null || this._gameAlloc == null) {
            throw new InvalidOperationException();
        }

        return this._gameAlloc(size, IntPtr.Zero, this._getGameAllocator(), IntPtr.Zero);
    }

    internal void Free(IntPtr ptr) {
        if (this._gameFree == null) {
            throw new InvalidOperationException();
        }

        this._gameFree(ptr);
    }
}
