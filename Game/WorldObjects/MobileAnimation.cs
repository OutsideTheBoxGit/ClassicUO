﻿using System;
using System.Collections.Generic;
using ClassicUO.AssetsLoader;

namespace ClassicUO.Game.WorldObjects
{
    public partial class Mobile
    {
        public Graphic GetGraphicForAnimation()
        {
            ushort g = Graphic;

            switch (g)
            {
                case 0x0192:
                case 0x0193:
                {
                    g -= 2;
                    break;
                }
            }

            return g;
        }

        public Direction GetDirectionForAnimation()
        {
            Direction dir = Direction & Direction.Up;

            if (_steps.Count > 0) dir = (Direction) _steps.Front().Direction & Direction.Up;
            return dir;
        }

        public void GetGroupForAnimation(in ANIMATION_GROUPS group, ref byte animation)
        {
            if ((sbyte) group > 0 && animation < (byte) PEOPLE_ANIMATION_GROUP.PAG_ANIMATION_COUNT)
                animation = _animAssociateTable[animation, (sbyte) group - 1];
        }

        public byte GetGroupForAnimation(in ushort checkGraphic = 0)
        {
            Graphic graphic = checkGraphic;
            if (graphic == 0)
                graphic = GetGraphicForAnimation();

            ANIMATION_GROUPS groupIndex = Animations.GetGroupIndex(graphic);
            byte result = AnimationGroup;

            if (result != 0xFF && (Serial & 0x80000000) <= 0 && (!AnimationFromServer || checkGraphic > 0))
            {
                GetGroupForAnimation(groupIndex, ref result);

                if (!Animations.AnimationExists(graphic, result))
                    CorrectAnimationGroup(graphic, groupIndex, ref result);
            }

            bool isWalking = IsWalking;
            bool isRun = IsRunning;

            if (_steps.Count > 0)
            {
                isWalking = true;
                isRun = _steps.Front().Run;
            }

            if (groupIndex == ANIMATION_GROUPS.AG_LOW)
            {
                if (isWalking)
                {
                    if (isRun)
                        result = (byte) LOW_ANIMATION_GROUP.LAG_RUN;
                    else
                        result = (byte) LOW_ANIMATION_GROUP.LAG_WALK;
                }
                else if (AnimationGroup == 0xFF)
                {
                    result = (byte) LOW_ANIMATION_GROUP.LAG_STAND;
                    AnimIndex = 0;
                }
            }
            else if (groupIndex == ANIMATION_GROUPS.AG_HIGHT)
            {
                if (isWalking)
                {
                    result = (byte) HIGHT_ANIMATION_GROUP.HAG_WALK;
                    if (isRun)
                        if (Animations.AnimationExists(graphic, (byte) HIGHT_ANIMATION_GROUP.HAG_FLY))
                            result = (byte) HIGHT_ANIMATION_GROUP.HAG_FLY;
                }
                else if (AnimationGroup == 0xFF)
                {
                    result = (byte) HIGHT_ANIMATION_GROUP.HAG_STAND;
                    AnimIndex = 0;
                }

                if (graphic == 151)
                    result++;
            }
            else if (groupIndex == ANIMATION_GROUPS.AG_PEOPLE)
            {
                bool inWar = InWarMode;

                if (isWalking)
                {
                    if (isRun)
                    {
                        if (Equipment[(int) Layer.Mount] != null)
                            result = (byte) PEOPLE_ANIMATION_GROUP.PAG_ONMOUNT_RIDE_FAST;
                        else if (Equipment[(int) Layer.LeftHand] != null || Equipment[(int) Layer.RightHand] != null)
                            result = (byte) PEOPLE_ANIMATION_GROUP.PAG_RUN_ARMED;
                        else
                            result = (byte) PEOPLE_ANIMATION_GROUP.PAG_RUN_UNARMED;

                        if (!IsHuman && !Animations.AnimationExists(graphic, result))
                        {
                            if (Equipment[(int) Layer.Mount] != null)
                            {
                                result = (byte) PEOPLE_ANIMATION_GROUP.PAG_ONMOUNT_RIDE_SLOW;
                            }
                            else if ((Equipment[(int) Layer.LeftHand] != null || Equipment[(int) Layer.RightHand] != null) && !IsDead)
                            {
                                if (inWar)
                                    result = (byte) PEOPLE_ANIMATION_GROUP.PAG_WALK_WARMODE;
                                else
                                    result = (byte) PEOPLE_ANIMATION_GROUP.PAG_WALK_ARMED;
                            }
                            else if (inWar && !IsDead)
                            {
                                result = (byte) PEOPLE_ANIMATION_GROUP.PAG_WALK_WARMODE;
                            }
                            else
                            {
                                result = (byte) PEOPLE_ANIMATION_GROUP.PAG_WALK_UNARMED;
                            }
                        }
                    }
                    else
                    {
                        if (Equipment[(int) Layer.Mount] != null)
                        {
                            result = (byte) PEOPLE_ANIMATION_GROUP.PAG_ONMOUNT_RIDE_SLOW;
                        }
                        else if ((Equipment[(int) Layer.LeftHand] != null || Equipment[(int) Layer.RightHand] != null) && !IsDead)
                        {
                            if (inWar)
                                result = (byte) PEOPLE_ANIMATION_GROUP.PAG_WALK_WARMODE;
                            else
                                result = (byte) PEOPLE_ANIMATION_GROUP.PAG_WALK_ARMED;
                        }
                        else if (inWar && !IsDead)
                        {
                            result = (byte) PEOPLE_ANIMATION_GROUP.PAG_WALK_WARMODE;
                        }
                        else
                        {
                            result = (byte) PEOPLE_ANIMATION_GROUP.PAG_WALK_UNARMED;
                        }
                    }
                }
                else if (AnimationGroup == 0xFF)
                {
                    if (Equipment[(int) Layer.Mount] != null)
                    {
                        result = (byte) PEOPLE_ANIMATION_GROUP.PAG_ONMOUNT_STAND;
                    }
                    else if (inWar && !IsDead)
                    {
                        if (Equipment[(int) Layer.LeftHand] != null)
                            result = (byte) PEOPLE_ANIMATION_GROUP.PAG_STAND_ONEHANDED_ATTACK;
                        else if (Equipment[(int) Layer.RightHand] != null)
                            result = (byte) PEOPLE_ANIMATION_GROUP.PAG_STAND_TWOHANDED_ATTACK;
                        else
                            result = (byte) PEOPLE_ANIMATION_GROUP.PAG_STAND_ONEHANDED_ATTACK;
                    }
                    else
                    {
                        result = (byte) PEOPLE_ANIMATION_GROUP.PAG_STAND;
                    }

                    AnimIndex = 0;
                }

                if (Race == RaceType.GARGOYLE)
                    if (IsFlying)
                    {
                        if (result == 0 || result == 1)
                            result = 62;
                        else if (result == 2 || result == 3)
                            result = 63;
                        else if (result == 4)
                            result = 64;
                        else if (result == 6)
                            result = 66;
                        else if (result == 7 || result == 8)
                            result = 65;
                        else if (result >= 9 && result <= 11)
                            result = 71;
                        else if (result >= 12 && result <= 14)
                            result = 72;
                        else if (result == 15)
                            result = 62;
                        else if (result == 20)
                            result = 77;
                        else if (result == 31)
                            result = 71;
                        else if (result == 34)
                            result = 78;
                        else if (result >= 200 && result <= 259)
                            result = 75;
                        else if (result >= 260 && result <= 270) result = 75;
                    }
            }

            return result;
        }


        private static void CorrectAnimationGroup(in ushort graphic, in ANIMATION_GROUPS group, ref byte animation)
        {
            if (group == ANIMATION_GROUPS.AG_LOW)
            {
                switch ((LOW_ANIMATION_GROUP) animation)
                {
                    case LOW_ANIMATION_GROUP.LAG_DIE_2:
                        animation = (byte) LOW_ANIMATION_GROUP.LAG_DIE_1;
                        break;
                    case LOW_ANIMATION_GROUP.LAG_FIDGET_2:
                        animation = (byte) LOW_ANIMATION_GROUP.LAG_FIDGET_1;
                        break;
                    case LOW_ANIMATION_GROUP.LAG_ATTACK_3:
                    case LOW_ANIMATION_GROUP.LAG_ATTACK_2:
                        animation = (byte) LOW_ANIMATION_GROUP.LAG_ATTACK_1;
                        break;
                }

                if (!Animations.AnimationExists(graphic, animation))
                    animation = (byte) LOW_ANIMATION_GROUP.LAG_STAND;
            }
            else if (group == ANIMATION_GROUPS.AG_HIGHT)
            {
                switch ((HIGHT_ANIMATION_GROUP) animation)
                {
                    case HIGHT_ANIMATION_GROUP.HAG_DIE_2:
                        animation = (byte) HIGHT_ANIMATION_GROUP.HAG_DIE_1;
                        break;
                    case HIGHT_ANIMATION_GROUP.HAG_FIDGET_2:
                        animation = (byte) HIGHT_ANIMATION_GROUP.HAG_FIDGET_1;
                        break;
                    case HIGHT_ANIMATION_GROUP.HAG_ATTACK_3:
                    case HIGHT_ANIMATION_GROUP.HAG_ATTACK_2:
                        animation = (byte) HIGHT_ANIMATION_GROUP.HAG_ATTACK_1;
                        break;
                    case HIGHT_ANIMATION_GROUP.HAG_MISC_4:
                    case HIGHT_ANIMATION_GROUP.HAG_MISC_3:
                    case HIGHT_ANIMATION_GROUP.HAG_MISC_2:
                        animation = (byte) HIGHT_ANIMATION_GROUP.HAG_MISC_1;
                        break;
                }

                if (!Animations.AnimationExists(graphic, animation))
                    animation = (byte) HIGHT_ANIMATION_GROUP.HAG_STAND;
            }
        }

        public static byte GetReplacedObjectAnimation(in Mobile mobile, in ushort index)
        {
            ushort getReplacedGroup(in IReadOnlyList<Tuple<ushort, byte>> list, in ushort idx, in ushort walkIdx)
            {
                foreach (Tuple<ushort, byte> item in list)
                    if (item.Item1 == idx)
                    {
                        if (item.Item2 == 0xFF)
                            return walkIdx;
                        return item.Item2;
                    }

                return idx;
            }

            ANIMATION_GROUPS group = Animations.GetGroupIndex(mobile.Graphic);

            if (group == ANIMATION_GROUPS.AG_LOW)
                return (byte) (getReplacedGroup(Animations.GroupReplaces[0], index, (ushort) LOW_ANIMATION_GROUP.LAG_WALK) % (ushort) LOW_ANIMATION_GROUP.LAG_ANIMATION_COUNT);
            if (group == ANIMATION_GROUPS.AG_PEOPLE)
                return (byte) (getReplacedGroup(Animations.GroupReplaces[1], index, (ushort) PEOPLE_ANIMATION_GROUP.PAG_WALK_UNARMED) % (ushort) PEOPLE_ANIMATION_GROUP.PAG_ANIMATION_COUNT);

            return (byte) (index % (ushort) HIGHT_ANIMATION_GROUP.HAG_ANIMATION_COUNT);
        }

        public static byte GetObjectNewAnimation(in Mobile mobile, in ushort type, in ushort action, in byte mode)
        {
            if (mobile.Graphic >= Animations.MAX_ANIMATIONS_DATA_INDEX_COUNT)
                return 0;

            switch (type)
            {
                case 0:
                    return GetObjectNewAnimationType_0(mobile, action, mode);
                case 1:
                case 2:
                    return GetObjectNewAnimationType_1_2(mobile, action, mode);
                case 3:
                    return GetObjectNewAnimationType_3(mobile, action, mode);
                case 4:
                    return GetObjectNewAnimationType_4(mobile, action, mode);
                case 5:
                    return GetObjectNewAnimationType_5(mobile, action, mode);
                case 6:
                case 14:
                    return GetObjectNewAnimationType_6_14(mobile, action, mode);
                case 7:
                    return GetObjectNewAnimationType_7(mobile, action, mode);
                case 8:
                    return GetObjectNewAnimationType_8(mobile, action, mode);
                case 9:
                case 10:
                    return GetObjectNewAnimationType_9_10(mobile, action, mode);
                case 11:
                    return GetObjectNewAnimationType_11(mobile, action, mode);
            }

            return 0;
        }

        private static byte GetObjectNewAnimationType_0(in Mobile mobile, in ushort action, in byte mode)
        {
            if (action <= 10)
            {
                ref IndexAnimation ia = ref Animations.DataIndex[mobile.Graphic];
                ANIMATION_GROUPS_TYPE type = ANIMATION_GROUPS_TYPE.MONSTER;

                if ((ia.Flags & 0x80000000) != 0)
                    type = ia.Type;

                if (type == ANIMATION_GROUPS_TYPE.MONSTER)
                {
                    switch (mode % 4)
                    {
                        case 1:
                            return 5;
                        case 2:
                            return 6;
                        case 3:
                            if ((ia.Flags & 1) != 0)
                                return 12;
                            goto case 0;
                        case 0:
                            return 4;
                    }
                }
                else if (type == ANIMATION_GROUPS_TYPE.SEA_MONSTER)
                {
                    if (mode % 2 != 0)
                        return 6;

                    return 5;
                }
                else if (type == ANIMATION_GROUPS_TYPE.ANIMAL)
                {
                    if (mobile.Equipment[(int) Layer.Mount] != null)
                    {
                        if (action > 0)
                        {
                            if (action == 1)
                                return 27;
                            if (action == 2)
                                return 28;

                            return 26;
                        }

                        return 29;
                    }

                    switch (action)
                    {
                        default:
                            return 31;
                        case 1:
                            return 18;
                        case 2:
                            return 19;
                        case 6:
                            return 12;
                        case 7:
                            return 13;
                        case 8:
                            return 14;
                        case 3:
                            return 11;
                        case 4:
                            return 9;
                        case 5:
                            return 10;
                    }
                }

                if (mode % 2 != 0)
                    return 6;

                return 5;
            }

            return 0;
        }

        private static byte GetObjectNewAnimationType_1_2(in Mobile mobile, in ushort action, in byte mode)
        {
            ref IndexAnimation ia = ref Animations.DataIndex[mobile.Graphic];
            ANIMATION_GROUPS_TYPE type = ANIMATION_GROUPS_TYPE.MONSTER;

            if ((ia.Flags & 0x80000000) != 0)
                type = ia.Type;

            if (type != ANIMATION_GROUPS_TYPE.MONSTER)
            {
                if (type <= ANIMATION_GROUPS_TYPE.ANIMAL || mobile.Equipment[(int) Layer.Mount] != null)
                    return 0xFF;
                return 30;
            }

            if (mode % 2 != 0) return 15;

            return 16;
        }

        private static byte GetObjectNewAnimationType_3(in Mobile mobile, in ushort action, in byte mode)
        {
            ref IndexAnimation ia = ref Animations.DataIndex[mobile.Graphic];
            ANIMATION_GROUPS_TYPE type = ANIMATION_GROUPS_TYPE.MONSTER;

            if ((ia.Flags & 0x80000000) != 0)
                type = ia.Type;

            if (type != ANIMATION_GROUPS_TYPE.MONSTER)
            {
                if (type == ANIMATION_GROUPS_TYPE.SEA_MONSTER) return 8;

                if (type == ANIMATION_GROUPS_TYPE.ANIMAL)
                {
                    if (mode % 2 != 0)
                        return 21;
                    return 22;
                }

                if (mode % 2 != 0)
                    return 8;
                return 12;
            }

            if (mode % 2 != 0) return 2;

            return 3;
        }

        private static byte GetObjectNewAnimationType_4(in Mobile mobile, in ushort action, in byte mode)
        {
            ref IndexAnimation ia = ref Animations.DataIndex[mobile.Graphic];
            ANIMATION_GROUPS_TYPE type = ANIMATION_GROUPS_TYPE.MONSTER;

            if ((ia.Flags & 0x80000000) != 0)
                type = ia.Type;

            if (type != ANIMATION_GROUPS_TYPE.MONSTER)
            {
                if (type > ANIMATION_GROUPS_TYPE.ANIMAL)
                {
                    if (mobile.Equipment[(int) Layer.Mount] != null)
                        return 0xFF;
                    return 20;
                }

                return 7;
            }

            return 10;
        }

        private static byte GetObjectNewAnimationType_5(in Mobile mobile, in ushort action, in byte mode)
        {
            ref IndexAnimation ia = ref Animations.DataIndex[mobile.Graphic];
            ANIMATION_GROUPS_TYPE type = ANIMATION_GROUPS_TYPE.MONSTER;

            if ((ia.Flags & 0x80000000) != 0)
                type = ia.Type;

            if (type <= ANIMATION_GROUPS_TYPE.SEA_MONSTER)
            {
                if (mode % 2 != 0)
                    return 18;

                return 17;
            }

            if (type != ANIMATION_GROUPS_TYPE.ANIMAL)
            {
                if (mobile.Equipment[(int) Layer.Mount] != null)
                    return 0xFF;

                if (mode % 2 != 0)
                    return 6;

                return 5;
            }

            switch (mode % 3)
            {
                case 1:
                    return 10;
                case 2:
                    return 3;
            }

            return 9;
        }

        private static byte GetObjectNewAnimationType_6_14(in Mobile mobile, in ushort action, in byte mode)
        {
            ref IndexAnimation ia = ref Animations.DataIndex[mobile.Graphic];
            ANIMATION_GROUPS_TYPE type = ANIMATION_GROUPS_TYPE.MONSTER;

            if ((ia.Flags & 0x80000000) != 0)
                type = ia.Type;

            if (type != ANIMATION_GROUPS_TYPE.MONSTER)
            {
                if (type != ANIMATION_GROUPS_TYPE.SEA_MONSTER)
                {
                    if (type == ANIMATION_GROUPS_TYPE.ANIMAL)
                        return 3;
                    if (mobile.Equipment[(int) Layer.Mount] != null)
                        return 0xFF;
                    return 34;
                }

                return 5;
            }

            return 11;
        }

        private static byte GetObjectNewAnimationType_7(in Mobile mobile, in ushort action, in byte mode)
        {
            if (mobile.Equipment[(int) Layer.Mount] != null)
                return 0xFF;

            if (action > 0)
            {
                if (action == 1)
                    return 33;
            }
            else
            {
                return 32;
            }

            return 0;
        }

        private static byte GetObjectNewAnimationType_8(in Mobile mobile, in ushort action, in byte mode)
        {
            ref IndexAnimation ia = ref Animations.DataIndex[mobile.Graphic];
            ANIMATION_GROUPS_TYPE type = ANIMATION_GROUPS_TYPE.MONSTER;

            if ((ia.Flags & 0x80000000) != 0)
                type = ia.Type;

            if (type != ANIMATION_GROUPS_TYPE.MONSTER)
            {
                if (type != ANIMATION_GROUPS_TYPE.SEA_MONSTER)
                {
                    if (type == ANIMATION_GROUPS_TYPE.ANIMAL)
                        return 9;
                    return mobile.Equipment[(int) Layer.Mount] != null ? (byte) 0xFF : (byte) 33;
                }

                return 3;
            }

            return 11;
        }

        private static byte GetObjectNewAnimationType_9_10(in Mobile mobile, in ushort action, in byte mode)
        {
            ref IndexAnimation ia = ref Animations.DataIndex[mobile.Graphic];
            ANIMATION_GROUPS_TYPE type = ANIMATION_GROUPS_TYPE.MONSTER;

            if ((ia.Flags & 0x80000000) != 0)
                type = ia.Type;

            return type != ANIMATION_GROUPS_TYPE.MONSTER ? (byte) 0xFF : (byte) 20;
        }

        private static byte GetObjectNewAnimationType_11(in Mobile mobile, in ushort action, in byte mode)
        {
            ref IndexAnimation ia = ref Animations.DataIndex[mobile.Graphic];
            ANIMATION_GROUPS_TYPE type = ANIMATION_GROUPS_TYPE.MONSTER;

            if ((ia.Flags & 0x80000000) != 0)
                type = ia.Type;

            if (type != ANIMATION_GROUPS_TYPE.MONSTER)
            {
                if (type >= ANIMATION_GROUPS_TYPE.ANIMAL)
                {
                    if (mobile.Equipment[(int) Layer.Mount] != null)
                        return 0xFF;
                    switch (action)
                    {
                        case 1:
                        case 2:
                            return 17;
                    }

                    return 16;
                }

                return 5;
            }

            return 12;
        }
    }
}