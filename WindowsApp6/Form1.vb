Public Class Form1
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        '用于显示计算耗时
        Dim sw As New Stopwatch()

        '种群规模
        Dim NP As Long = 200
        '遗传代数
        Dim G As Long = 1000
        '交叉率
        Dim Pc As Double = 0.8
        '变异率
        Dim Pm As Double = 0.1

        '初始化数据
        Initializing_Data()

        sw.Start()

        '使用遗传算法求得重新排序后的最优化布置的车重数组，同时更新FieldVehicles的相关参数
        Dim outListOfNumFV = GA_Vehicles_Layout(StandardVehicles, FieldVehicles, NP, G, Pc, Pm)

        '结果可视化展示
        Dim x As New List(Of Long)
        For index = 0 To G - 1
            x.Add(index)
        Next

        '图表相关
        Me.Text = "使用GA优化车辆布置形式"
        Chart1.ChartAreas.Item(0).AxisX.Title = "迭代次数"
        Chart1.ChartAreas.Item(0).AxisX.IntervalAutoMode = DataVisualization.Charting.IntervalAutoMode.VariableCount
        Chart1.ChartAreas.Item(0).AxisY.Title = "目标函数值"
        Chart1.Series(0).ChartType = DataVisualization.Charting.SeriesChartType.Line
        Chart1.Series(0).Points.DataBindXY(x, outListOfNumFV)
        Chart1.Series(0).IsVisibleInLegend = False

        '展示参数到窗口
        tbNP.Text = NP
        tbG.Text = G
        tbPc.Text = Pc
        tbPm.Text = Pm
        tbMinF.Text = outListOfNumFV.Min()

        '初始化表格
        Initializing_DataGridView()
        '将FieldVehicles数据显示在DataGridView中
        Dim standardVehicleWeight As Double
        For index = 0 To FieldVehicles.Count - 1
            Dim f = FieldVehicles.ElementAt(index)
            '在StandardVehicles查找Weight匹配项
            Dim result = StandardVehicles.Find(Function(tempt) tempt.i = FieldVehicles.ElementAt(index).i And tempt.j = FieldVehicles.ElementAt(index).j)
            '处理匹配失败的情况
            standardVehicleWeight = If(result Is Nothing, Nothing, result.Weight)
            '添加行
            DataGridView1.Rows.Add(f.name, f.i, f.j, f.Weight, standardVehicleWeight)
        Next

        '计算时长
        sw.Stop() : Dim ts = sw.Elapsed
        Debug.WriteLine("合计用时" + ts.TotalMilliseconds.ToString() + "ms")

    End Sub

    '初始化表格
    Private Sub Initializing_DataGridView()
        DataGridView1.RowHeadersVisible = False
        DataGridView1.ColumnCount = 5
        DataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        DataGridView1.Columns(0).HeaderText = "名称"
        DataGridView1.Columns(1).HeaderText = "行号"
        DataGridView1.Columns(2).HeaderText = "列号"
        DataGridView1.Columns(3).HeaderText = "车重"
        DataGridView1.Columns(4).HeaderText = "标准车重"
        For i = 0 To DataGridView1.ColumnCount - 1
            DataGridView1.Columns(i).AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            DataGridView1.Columns(i).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        Next
        DataGridView1.Columns(DataGridView1.ColumnCount - 1).AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
    End Sub
End Class
