var block = new Vue({
    el: '#SearchBlock',
    data: {
        apiLink: [
            "/api/parse/GetPriceFromDate",
            "/api/parse/GetAvgPriceFromDateToDate",
            "/api/parse/GetMinMaxPriceFromDateToDate",
            "/api/parse/GetStat"
        ],
        search: 0,
        getPriceFromDateValue: null,
        getPriceFromDateResult: 0,
        getAvgPriceFromDateValue: null,
        getAvgPriceToDateValue: null,
        getAvgPriceFromDateToDateResult: 0,
        getMinMaxPriceFromDateValue: null,
        GetMinMaxPriceToDateValue: null,
        GetMinMaxPriceFromDateToDateResult: {
            min: 0,
            max: 0
        },
        getStatCount: 0,
        getStatMinMax: {
            min: 0,
            max: 0
        },
        getStatMinDate: null,
        getStatMaxDate: null,
        getStatAvg: 0
    },
    methods: {
        getPriceFromDateFunc() {
            if (this.getPriceFromDateValue == null) return;
            axios.get(block.apiLink[0], {
                params: {
                    date: block.getPriceFromDateValue
                }
            })
            .then(function (response) {
                block.getPriceFromDateResult = response.data;
            })
            .catch(function (error) {
                console.log(error);
            });
        },
        getAvgPriceFromDateToDate() {
            if (this.getAvgPriceFromDateValue == null ||
                this.getAvgPriceToDateValue == null) return;
            axios.get(block.apiLink[1], {
                params: {
                    from: block.getAvgPriceFromDateValue,
                    to: block.getAvgPriceToDateValue
                }
            })
            .then(function (response) {
                block.getAvgPriceFromDateToDateResult = response.data;
            })
            .catch(function (error) {
                console.log(error);
            });
        },
        getMinMaxPriceFromDateToDate() {
            if (this.getMinMaxPriceFromDateValue == null ||
                this.GetMinMaxPriceToDateValue == null) return;
            axios.get(block.apiLink[2], {
                params: {
                    from: block.getMinMaxPriceFromDateValue,
                    to: block.GetMinMaxPriceToDateValue
                }
            })
            .then(function (response) {
                block.GetMinMaxPriceFromDateToDateResult.min = response.data.Min;
                block.GetMinMaxPriceFromDateToDateResult.max = response.data.Max;
            })
            .catch(function (error) {
                console.log(error);
            });
        },
        getStat() {
            axios.get(block.apiLink[3])
            .then(function (response) {
                var data = response.data;
                block.getStatCount = data.RowCount;
                block.getStatMinMax.min = data.MinMaxValue.Min;
                block.getStatMinMax.max = data.MinMaxValue.Max;
                block.getStatMinDate = data.MinDate;
                block.getStatMaxDate = data.MaxDate;
                block.getStatAvg = data.AvgValue;
            })
            .catch(function (error) {
                console.log(error);
            });
        }
    }
});

block.getStat();