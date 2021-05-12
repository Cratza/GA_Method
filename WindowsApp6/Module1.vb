Imports System.Math

Public Class cls_GA_Vehicle_Info
    Public name As String = ""
    Public i As Long = 0
    Public j As Long = 0
    Public Weight As Double = 0.0#
    Public Sub New(cName As String, cRow As Long, cCol As Long, cWeight As Double)
        name = cName : i = cRow : j = cCol : Weight = cWeight
    End Sub
End Class

Module Module1
    Public nRows As Long = 0
    Public nCols As Long = 0
    Public StandardVehicles As New List(Of cls_GA_Vehicle_Info)
    Public FieldVehicles As New List(Of cls_GA_Vehicle_Info)

    '默认随机种子
    Public iSeed As Integer = 6
    '随机概率
    Public rand As New Random(iSeed)
    '储存优化函数的数组
    Public RSigmaF As New List(Of Double)


    Public Sub Initializing_Data()
        StandardVehicles.Clear()
        FieldVehicles.Clear()

        StandardVehicles.Add(New cls_GA_Vehicle_Info("S1", 1, 1, 121))
        StandardVehicles.Add(New cls_GA_Vehicle_Info("S2", 2, 1, 861))
        StandardVehicles.Add(New cls_GA_Vehicle_Info("S3", 3, 1, 168))
        StandardVehicles.Add(New cls_GA_Vehicle_Info("S4", 1, 2, 481))
        StandardVehicles.Add(New cls_GA_Vehicle_Info("S5", 2, 2, 184))
        StandardVehicles.Add(New cls_GA_Vehicle_Info("S6", 3, 2, 151))
        StandardVehicles.Add(New cls_GA_Vehicle_Info("S7", 1, 3, 848))
        StandardVehicles.Add(New cls_GA_Vehicle_Info("S8", 2, 3, 111))
        StandardVehicles.Add(New cls_GA_Vehicle_Info("S9", 3, 3, 585))
        StandardVehicles.Add(New cls_GA_Vehicle_Info("S10", 1, 4, 244))
        StandardVehicles.Add(New cls_GA_Vehicle_Info("S11", 1, 4, 313))

        '获取StandardVehicles的维度信息
        Call Get_Problem_Ranks(StandardVehicles)
        FieldVehicles.Add(New cls_GA_Vehicle_Info("F1", 0, 0, 384))
        FieldVehicles.Add(New cls_GA_Vehicle_Info("F2", 0, 0, 121))
        FieldVehicles.Add(New cls_GA_Vehicle_Info("F3", 0, 0, 870))
        FieldVehicles.Add(New cls_GA_Vehicle_Info("F4", 0, 0, 168))
        FieldVehicles.Add(New cls_GA_Vehicle_Info("F5", 0, 0, 484))
        FieldVehicles.Add(New cls_GA_Vehicle_Info("F6", 0, 0, 561))
        FieldVehicles.Add(New cls_GA_Vehicle_Info("F7", 0, 0, 184))
        FieldVehicles.Add(New cls_GA_Vehicle_Info("F8", 0, 0, 444))
        FieldVehicles.Add(New cls_GA_Vehicle_Info("F9", 0, 0, 813))
        FieldVehicles.Add(New cls_GA_Vehicle_Info("F10", 0, 0, 151))
        FieldVehicles.Add(New cls_GA_Vehicle_Info("F11", 0, 0, 848))
        FieldVehicles.Add(New cls_GA_Vehicle_Info("F12", 0, 0, 112))
        FieldVehicles.Add(New cls_GA_Vehicle_Info("F13", 0, 0, 585))
        FieldVehicles.Add(New cls_GA_Vehicle_Info("F14", 0, 0, 122))
        FieldVehicles.Add(New cls_GA_Vehicle_Info("F15", 0, 0, 861))

    End Sub

    Public Sub Get_Problem_Ranks(StandardVehicles As List(Of cls_GA_Vehicle_Info))
        nRows = 0 : nCols = 0
        For Each v As cls_GA_Vehicle_Info In StandardVehicles
            Dim ci As Long = v.i
            Dim cj As Long = v.j
            nRows = Math.Max(nRows, cj)
            nCols = Math.Max(nCols, cj)
        Next
    End Sub

    '采用遗传算法更新 FieldVehicles 中各个车辆的i和j
    '输入参数：P--标准车重列表，
    '         Q--场地车重列表，
    '         NP--种群规模，
    '         G--迭代次数,
    '         Pc--交叉率,
    '         Pm--变异率
    '输出参数：优化函数变化数组
    '         ’优化后的场地车重数组
    Public Function GA_Vehicles_Layout(ByRef StandardVehicles As List(Of cls_GA_Vehicle_Info), ByRef FieldVehicles As List(Of cls_GA_Vehicle_Info), ByVal NP As Long, ByVal G As Long, ByVal Pc As Double, ByVal Pm As Double) As List(Of Double)
        '将StandardVehicles转换为只包含车重的列表
        Dim P = AllList2WeightList(StandardVehicles)
        '将FieldVehicles转换为只包含车重的列表
        Dim Q = AllList2WeightList(FieldVehicles)
        '===============================================
        '               Step1.初始化数据
        '===============================================
        '基因个数，即每个个体的参数个数
        Dim N As Long = P.Count
        '场地车辆个数
        Dim Ns As Long = Q.Count
        '储存种群的二维数组
        Dim f As New List(Of Double)
        '储存种群更新中间数据的二维数组
        Dim nf As New List(Of Double)
        '储存个体的一维数组
        Dim tempF As New List(Of Double)
        '储存序列的临时索引数组
        Dim randIndex As New List(Of Integer)
        '储存最优布置的数组
        Dim R As New List(Of Double)
        '储存每个个体的待优化函数
        Dim sigmaF As New List(Of Double)
        '优化函数的最大、最小函数值
        Dim maxSigmaF, minSigmaF As Double
        '最小函数值个体所在位置索引
        Dim indexMinSigmaF As Integer
        '储存每个个体的归一化适应值
        Dim fitness As New List(Of Double)
        '当前代数
        Dim gen As Integer = 0
        '子代中的两个个体A、B
        Dim A, B As New List(Of Double)
        '待交换的基因位置序号
        Dim p1, p2 As Integer
        '临时变量
        Dim temp As Double
        'Dim tempQ As New List(Of Double)
        '将Q保存到基因库，方便后续取用
        Dim Qc As New List(Of Double)(Q)
        '取Q前N个生成子代基因
        Q = Q.Take(N).ToList()
        '随机生成初代种群
        For i = 0 To NP - 1
            tempF.Clear()
            randIndex = MyRandperm(N)
            For j = 0 To randIndex.Count - 1
                tempF.Add(Q.ElementAt(randIndex.ElementAt(j) - 1))
            Next
            f.AddRange(tempF)
        Next
        '存储最优布置
        R = f.Take(N).ToList()
        '达到指定代数后结束遗传算法
        While gen < G
            '初始化临时种群
            nf.Clear()
            sigmaF.Clear()
            fitness.Clear()
            '===============================================
            '                Step2.个体评价
            '===============================================
            For i = 0 To NP - 1
                tempF.Clear()
                tempF = f.GetRange(i * N, N)
                '计算每种布置的优化函数计算值
                sigmaF.Add(GAFunction(tempF, P))
            Next
            maxSigmaF = sigmaF.Max()
            minSigmaF = sigmaF.Min()
            '使用LinQ查询最小值所在位置索引，若存在多个最小值，则返回第一个最小值的索引
            indexMinSigmaF = sigmaF.FindIndex(Function(tempt) tempt = minSigmaF)
            '保存最优的个体
            R = f.GetRange(indexMinSigmaF * N, N)
            '计算归一化适应值
            For i = 0 To sigmaF.Count - 1
                fitness.Add(1 - ((sigmaF(i) - minSigmaF) / (maxSigmaF - minSigmaF + 0.001)))
            Next
            '===============================================
            '                Step3.选择操作
            '===============================================
            '通过选择的个体数量
            Dim nn As Integer = 0
            For i = 0 To NP - 1
                If fitness(i) >= rand.NextDouble() Then
                    nn += 1
                    nf.AddRange(f.GetRange(i * N, N))
                End If
            Next
            '===============================================
            '                Step4.交叉操作
            '===============================================
            '交叉点个数w，与交叉率Pc相关
            Dim crossNum As Integer = Ceiling(N / Ceiling(Pc * 10))
            '变异点个数w，与变异率Pm相关
            Dim mutationNum As Integer = Ceiling(Pm * N)
            Dim randP As Integer
            Dim x, y As Double
            While nn < NP
                '随机选择两个个体A、B
                A = nf.GetRange(rand.Next(nn) * N, N)
                B = nf.GetRange(rand.Next(nn) * N, N)
                randP = rand.Next(N - crossNum + 1)

                For i = 1 To crossNum
                    x = A.FindIndex(Function(tempt) tempt = B(randP + i - 1))
                    y = B.FindIndex(Function(tempt) tempt = A(randP + i - 1))

                    '处理查询失败的情况
                    If x = -1 Or y = -1 Then Continue For

                    temp = A(randP + i - 1)
                    A(randP + i - 1) = B(randP + i - 1)
                    B(randP + i - 1) = temp

                    temp = A(x) : A(x) = B(y) : B(y) = temp
                Next
                '===============================================
                '                Step5.变异操作
                '===============================================
                For index = 1 To mutationNum
                    '生成0~(N-1)的随机整数p1、p2
                    p1 = Math.Floor(N * rand.NextDouble())
                    p2 = Math.Floor(Ns * rand.NextDouble())

                    '从基因库中获取变异基因
                    A(p1) = Qc(p2) : B(p1) = Qc(p2)

                    '将变异后的子代放入nf子代
                    nf.AddRange(A)
                    nf.AddRange(B)
                Next
                nn = nf.Count / N
            End While
            '当后代超过种群数量时，选择前NP个元素作为后代
            If nn > NP Then nf = nf.GetRange(0, NP * N)
            '更新种群，此处将nf的值传递到f，直接引用赋值会导致后续结果出错
            f = New List(Of Double)(nf)
            '保留每代最优个体
            For i = 0 To N - 1
                f(i) = R(i)
            Next
            '将最小值保存到RSigmaF数组
            RSigmaF.Add(minSigmaF)
            '更新代数
            gen += 1
        End While
        '===============================================
        '            Step6.更新FieldVehicles的布置
        '===============================================
        For num = 0 To R.Count - 1
            temp = Qc.FindIndex(Function(tempt) tempt = R(num))
            With FieldVehicles.ElementAt(temp)
                .Weight = R(num)
                .i = StandardVehicles.ElementAt(num).i
                .j = StandardVehicles.ElementAt(num).j
            End With
        Next
        Return RSigmaF
    End Function

    '待优化函数：∑(Q_i-P_i)^2
    Function GAFunction(ByVal Q As List(Of Double), ByVal P As List(Of Double)) As Double
        Dim numQ = Q.Count
        Dim Sigma As Double = 0
        For i = 0 To numQ - 1
            Sigma += Math.Pow((Q.ElementAt(i) - P.ElementAt(i)), 2)
        Next
        Return Sigma
    End Function

    '将cls_GA_Vehicle_Info类型的数组转换为Double类型的数组
    Function AllList2WeightList(ByVal list As List(Of cls_GA_Vehicle_Info)) As List(Of Double)
        Dim arrNum As New List(Of Double)
        Dim count As Integer = 1
        For Each c As cls_GA_Vehicle_Info In list
            arrNum.Add(c.Weight)
            count += 1
        Next
        Return arrNum
    End Function

    '随机生成长度为Num的1~Num的乱序一维整型数组
    Function MyRandperm(ByVal Num As Integer) As List(Of Integer)
        Dim list As New List(Of Integer)
        Dim index, temp As Integer
        '按照顺序生成1~Num的数组list
        For i = 1 To Num
            list.Add(i)
        Next
        '将数组顺序打乱
        For i = 1 To Num
            index = rand.Next(i)
            temp = list(i - 1)
            '随机交换数据
            list(i - 1) = list(index)
            list(index) = temp
        Next
        Return list
    End Function

    ''为数组排序，并返回索引数组
    'Function SortListWithIndex(ByRef list As List(Of Double)) As List(Of Integer)
    '    Dim lstOrg As New List(Of Double)
    '    Dim lstSort As New List(Of Double)
    '    lstOrg.AddRange(list)
    '    lstSort.AddRange(list)
    '    lstSort.Sort()

    '    Dim lstIndex As New List(Of Integer)
    '    For i = 0 To lstSort.Count - 1
    '        Dim index As Integer = lstOrg.IndexOf(lstSort(i))
    '        While lstIndex.IndexOf(index) >= 0
    '            index = lstOrg.IndexOf(lstSort(i), index + 1)
    '        End While
    '        lstIndex.Add(index)
    '    Next
    '    Return lstIndex
    'End Function

End Module
