<template>
  <div>
    <v-card class="elevation-12"  v-if="!submitted">
      <v-toolbar color="primary" dark flat>
        <v-toolbar-title>Payment Form </v-toolbar-title>
        <v-spacer></v-spacer>
      </v-toolbar>
      <v-card-text>
        <p> <b>Full transaction value : </b> {{ value }} {{ currency }} </p>
        <v-form>
          <v-text-field :rules="validation.name" label="Full Name" name="cardHolder" v-model="cardHolder" prepend-icon="mdi-account" type="text" required></v-text-field>
          <v-text-field :rules="validation.card" label="Card Number" name="cardNumber" v-model="cardNumber" prepend-icon="mdi-card" type="tel" placeholder="#### #### #### ####" maxlength="16" v-cardformat:formatCardNumber required></v-text-field>
          <v-row>
            <v-col cols="6" sm="12" md="6">
                <v-select :rules="validation.expiryMonth" label="Expires (Month)" :items="months" name="expiryMonth" v-model="expiryMonth" prepend-icon="mdi-calendar" required></v-select>
            </v-col>
            <v-col cols="6" sm="12" md="6">
                <v-select :rules="validation.expiryYear" label="Expires (Year)" :items="years" name="expiryYear" v-model="expiryYear" prepend-icon="mdi-calendar" required></v-select>
            </v-col>
            
            <v-col cols="12" sm="12" md="12">
                <v-text-field :rules="validation.ccv" label="CCV" name="ccv" v-model="ccv" prepend-icon="mdi-lock" type="password" v-cardformat:formatCardCVC required></v-text-field>
            </v-col>
          </v-row>
        </v-form>
      </v-card-text>
      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn color="green" v-on:click="confirmPayment">Pay</v-btn>
      </v-card-actions>
    </v-card>

    <v-card class="elevation-12"  v-if="submitted && successful">
      <v-toolbar color="green" dark flat>
        <v-toolbar-title>Payment successful </v-toolbar-title>
        <v-spacer></v-spacer>
      </v-toolbar>
    </v-card>

    <v-card class="elevation-12"  v-if="submitted && !successful">
      <v-toolbar color="red" dark flat>
        <v-toolbar-title>Payment was not successful </v-toolbar-title>
        <v-spacer></v-spacer>
      </v-toolbar>

      <v-card-text v-if="submitted && !successful && hasError">
        Service unavailabe, please try again later!
      </v-card-text>
      <v-card-text v-if="submitted && !successful && !hasError">
        Transaction was rejected. Please try again or contact your credit card provider!
      </v-card-text>
    </v-card>
  </div>
</template>
<script>
export default {
  name: "PaymentForm",
  props: {
    value: Number,
    currency: String,
    entity: String,
    apiKey: String,
  },
  data: () => ({
    validation: {
      card: [
        v => !!v || 'Card number is required',
        v => new RegExp(`^(1298|1267|4512|4567|8901|8933)([]?[0-9]{4}){3}$`).test(v) || "Please insert a valid Credit Card Nubmer"
      ],
      name: [v => !!v || 'Name is required'],
      expiryYear: [v => !!v || 'Expire Year is required'],
      expiryMonth: [v => !!v || 'Expire Month is required'],
      ccv: [
        v => !!v || 'CCV is required',
        v => v.lenght() !== 3 || 'The CCV should be 3 numeric digits'
      ],

    },


    months: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12],
    years: [2020, 2021, 2022, 2023, 2024, 2025, 2026, 2027, 2028, 2029, 2030],
    submitted: false,
    successful: false,
    hasError: false,
    cardNumber: '',
    cardHolder: '',
    expiryMonth: new Date(Date.now()).getMonth() + 1,
    expiryYear: new Date(Date.now()).getFullYear(),
    ccv: ''
  }),
  computed: {



  },
  methods: {
    confirmPayment() {

      const payment = {
        issuer: this.entity,
        cardHolder: this.cardHolder,
        value: this.value,
        currency: this.currency,
        cardNumber: this.cardNumber,
        expiryMonth: this.expiryMonth,
        expiryYear: this.expiryYear,
        ccv: this.ccv,
      };

      const axiosConfiguration = {
          method: 'post',
          url: 'https://localhost:5001/payment',
          headers: {
            'X-Api-Key': this.apiKey,
          },
          data: payment,
      };
      
      this.axios(axiosConfiguration).then(response => {

        this.submitted = true;
        if (response.data.paymentResponse.successful) {
          this.successful = true;
        } else { this.successful = false }

      }).catch((err) => {
        console.log(err);
        this.submitted = true;
        this.successful = false;
        this.hasError = true;
      });
      
    }
  }
};
</script>
