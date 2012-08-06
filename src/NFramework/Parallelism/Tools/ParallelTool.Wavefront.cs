using System;
using System.Threading.Tasks;

namespace NSoft.NFramework.Parallelism.Tools {
    public static partial class ParallelTool {
        /// <summary>
        /// Matrix 연산 중에 현재 행보다 아래와 현재 열보다 오른쪽의 요소에 대해 작업을 Wavefront라 한다. 이를 병렬로 수행할 수 있도록 합니다.
        /// </summary>
        /// <param name="rows">매트릭스 행 수</param>
        /// <param name="cols">매트릭스 열 수</param>
        /// <param name="blocksPerRow">매트릭스 행 분할 수</param>
        /// <param name="blocksPerCol">매트릭스 열 분할 수</param>
        /// <param name="processBlock">매트릭스의 분할된 서브매트릭스를 처리할 델리게이트. processBlock(startRow, endRow, startCol, endCol);</param>
        public static void Wavefront(int rows, int cols, int blocksPerRow, int blocksPerCol, Action<int, int, int, int> processBlock) {
            rows.ShouldBePositive("rows");
            cols.ShouldBePositive("cols");
            processBlock.ShouldNotBeNull("processBlock");

            blocksPerRow.ShouldBeInRange(1, rows + 1, "blocksPerRow");
            blocksPerCol.ShouldBeInRange(1, cols + 1, "blocksPerCol");

            if(IsDebugEnabled)
                log.Debug("매트릭스의 블록 분할에 따른 Wavefront 작업을 수행합니다. rows=[{0}], cols=[{1}], blocksPerRow=[{2}], blocksPerCol=[{3}]",
                          rows, cols, blocksPerRow, blocksPerCol);

            int rowBlockSize = rows / blocksPerRow;
            int colBlockSize = cols / blocksPerCol;

            Wavefront(blocksPerRow,
                      blocksPerCol,
                      (r, c) => {
                          int startRow = r * rowBlockSize;
                          int endRow = (r < blocksPerRow - 1) ? startRow + rowBlockSize : rows;

                          int startCol = c * colBlockSize;
                          int endCol = (c < blocksPerCol - 1) ? startCol + colBlockSize : cols;

                          if(IsDebugEnabled)
                              log.Debug("매트릭스 서브 블럭 프로세싱... " +
                                        "r=[{0}], c=[{1}], rowBlockSize=[{2}], colBlockSize=[{3}], startRow=[{4}], endRow=[{5}], startCol=[{6}], endCol=[{7}]",
                                        r, c, rowBlockSize, colBlockSize, startRow, endRow, startCol, endCol);

                          processBlock(startRow, endRow, startCol, endCol);
                      });

            if(IsDebugEnabled)
                log.Debug("매트릭스의 블록 분할에 따른 모든 Wavefront 작업을 완료했습니다!!!");
        }

        /// <summary>
        /// 매트릭스에서 각 Cell의 연산을 Wavefront (물결)과 같이, 상위 Cell, 좌측 Cell의 작업이 완료한 후에, 작업하도록 합니다.
        /// </summary>
        /// <param name="rows">매트릭스 행 수</param>
        /// <param name="cols">매트릭스 열 수</param>
        /// <param name="processCell">매트릭스의 각 셀을 처리할 델리게이트</param>
        public static void Wavefront(int rows, int cols, Action<int, int> processCell) {
            rows.ShouldBePositive("rows");
            cols.ShouldBePositive("cols");
            processCell.ShouldNotBeNull("processCell");

            if(IsDebugEnabled)
                log.Debug("매트릭스 Wavefront 작업을 시작합니다. rows=[{0}], cols=[{1}]", rows, cols);

            try {
                // 현재 Cell의 상위 행 (r-1)의 열(column)별 작업 정보
                Task[] prevTaskRows = new Task[cols];
                // 현재 Cell의 바로 전 작업 (c-1)
                Task prevTaskInCurrentRow = null;
                // 현재 cell이 (r,c)일 때, (r-1, c), (r, c-1) 인 cell 의 작업을 나타낸다.
                var depedencies = new Task[2];

                for(var r = 0; r < rows; r++) {
                    prevTaskInCurrentRow = null;

                    for(var c = 0; c < cols; c++) {
                        int i = r, j = c;
                        Task currTask;

                        if(IsDebugEnabled)
                            log.Debug("매트릭스 Wavefront Task 정의 중... Matrix Cell({0},{1})", i, j);

                        if(r == 0 && c == 0) {
                            currTask = Task.Factory.StartNew(() => processCell(i, j));
                        }
                        else if(r == 0 || c == 0) {
                            // 전 작업
                            var antecedent = (c == 0) ? prevTaskRows[0] : prevTaskInCurrentRow;

                            currTask = antecedent.ContinueWith(p => {
                                                                   p.Wait(); // 예외 전파를 위해 필요할 뿐
                                                                   processCell(i, j);
                                                               },
                                                               TaskContinuationOptions.ExecuteSynchronously);
                        }
                        else {
                            depedencies[0] = prevTaskInCurrentRow; // 좌측 cell
                            depedencies[1] = prevTaskRows[c]; // 윗쪽 cell

                            currTask = Task.Factory.ContinueWhenAll(depedencies,
                                                                    ps => {
                                                                        Task.WaitAll(ps);
                                                                        processCell(i, j);
                                                                    },
                                                                    TaskContinuationOptions.ExecuteSynchronously);
                        }

                        if(IsDebugEnabled)
                            log.Debug("매트릭스 Wavefront Task 정의 완료!!! Matrix Cell({0},{1}), Task Id=[{2}]", i, j, currTask.Id);

                        prevTaskRows[c] = prevTaskInCurrentRow = currTask;
                    }
                }

                prevTaskInCurrentRow.Wait();
            }
            catch(AggregateException age) {
                if(log.IsErrorEnabled)
                    log.Error(age);

                age.Handle(ex => false);
            }

            if(IsDebugEnabled)
                log.Debug("매트릭스 Wavefront 작업이 완료되었습니다!!!");
        }
    }
}