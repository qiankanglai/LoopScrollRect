# Loop Scroll Rect

These scripts help make your ScrollRect `Reusable`, because it will only build cells when needed. If you have a large number of cells in a scroll rect, you absolutely need it! It will save a lot of time loading and draw call, along with memory in use, while still working smoothly.

中文说明请看[这里](http://qiankanglai.me/misc/2015/08/15/LoopScrollRect/)。

## Demo

Demo for Loop Scroll Rect. Each cell knows its index, and it is able to modify its content/size/color easily.

![Demo](Images/demo1.gif)

Demo without mask. As you can see, the cells are only instantiated when needed and recycled.

![Demo2](Images/demo2.gif)

## Introduction

The original code comes from @ivomarel's [InfinityScroll](https://github.com/ivomarel/InfinityScroll). I improved his code in several fields:
- Avoid using `sizeDelta` directly since it doesn't always mean size
- Support GridLayout
- Check Anchor and Pivot for convenience
- **KEY** Avoid blocking when dragging back
- Take advantage of pool rather than instantiate/destroy every time
- Improve some other details for performance

Also, I modified [Easy Object Pool](https://www.assetstore.unity3d.com/cn/#!/content/31928) for recycling the cells. 

Note: My scripts copies the `ScrollRect` from [UGUI](https://bitbucket.org/Unity-Technologies/ui) 5.1 rather than inherit `ScrollRect` like InfinityScroll, because I need to modify some private variants to make dragging smooth. All my codes is wrapped with comments like `==========LoopScrollRect==========`.

Note: If you need scroll infinitely, you can simply delete codes about `itemTypeStart`, `itemTypeEnd` in my scripts.

## Example: Loop Vertical Scroll Rect

These steps may be confusing, so you can just open the demo scene and copy & paste :D

You can also remove EasyObjPool and use your pool instead.

1. Prepare cell prefabs in `Resources` folder
    - The cell needs `Layout Element` attached and preferred width/height
    - You should add a script receiving message `void ScrollCellIndex (int idx) `
![ScrollCell](Images/ScrollCell.png)
2. Prepare EasyObjectPool with different cell prefabs
![EasyObjectPool](Images/EasyObjectPool.png)
3. Attach `LoopVerticalScrollRect` and `Mask` to the Scroll gameObject
	- Clear Cells: remove existing cells and keep uninitialized
	- Refill Cells: initialize and fill up cells
	- Init in Start: call Refill cells automatically when Start
	- Prefab Pool: the EasyObjPool gameObject
	- Prefab Pool Name: the corresponding pool in step 2
	- Total Count: How many cells are available (index: 0 ~ TotalCount-1)
	- Cache Extend Pixels: How many cells should be prepared before start or after end? If it is too small, it may bounce back when dragging. I highly suggests you modify it and simply try.
![LoopVerticalScrollRect](Images/LoopVerticalScrollRect.png)
4. Attach `Content Size Filter` and `Vertical Layout Group` to the Content gameObject.
![Content](Images/Content.png)